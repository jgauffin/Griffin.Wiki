﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Griffin.Logging;
using Griffin.Wiki.Core.DomainModels;
using Griffin.Wiki.Core.Repositories;
using Griffin.Wiki.Core.Services;
using Griffin.Wiki.WebClient.Models.Page;
using Helpers;
using Sogeti.Pattern.Data;
using Sogeti.Pattern.Mvc3.Data;

namespace Griffin.Wiki.WebClient.Controllers
{
    public class PageController : BaseController
    {
        private readonly IPageRepository _repository;
        private readonly PageService _pageService;
        private readonly IUserRepository _userRepository;
        private readonly TemplateRepository _templateRepository;
        private readonly PageTreeRepository _pageTreeRepository;

        public PageController(IPageRepository repository, PageService pageService, IUserRepository userRepository, TemplateRepository templateRepository, PageTreeRepository pageTreeRepository)
        {
            _repository = repository;
            _pageService = pageService;
            _userRepository = userRepository;
            _templateRepository = templateRepository;
            _pageTreeRepository = pageTreeRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        protected override void HandleUnknownAction(string actionName)
        {
            base.HandleUnknownAction(actionName);
        }

        public ActionResult Show(string id)
        {
            var page = _repository.Get(id);
            if (page == null)
            {
                return RedirectToAction("Create", new {id = id});
            }

            var tocBuilder = new TableOfContentsBuilder();
            tocBuilder.Compile(page.HtmlBody);

            var tree = _pageTreeRepository.Get(page.Id);

            var model = new ShowViewModel
                            {
                                Body = page.HtmlBody,
                                PageName = id,
                                Title = page.Title,
                                UpdatedAt = page.UpdatedAt,
                                UserName = page.UpdatedBy.DisplayName,
                                BackLinks = page.BackReferences.Select(k => k.PageName).ToList(),
                                TableOfContents = tocBuilder.GenerateList(),
                                Path = tree.MakePath(Url.Action("Show"))
                            };

            return View(model);
        }

        [Authorize]
        public ActionResult Edit(string id)
        {
            var page = _repository.Get(id);
            var model = new CreateViewModel {PageName = id, Title = page.Title, Content = page.RawBody};
            return View(model);
        }

        [HttpPost, Transactional2, Authorize]
        public ActionResult Edit(CreateViewModel model)
        {
            _pageService.UpdatePage(model.PageName, model.Title, model.Content);

            return RedirectToAction("Show", new {id = model.PageName});
        }

        [Authorize]
        public ActionResult Create(string id, string title = null, string parentName = null)
        {
            var model = new CreateViewModel
                            {
                                PageName = id,
                                Title = title ?? id,
                                Content = "",
                                Templates = new List<SelectListItem>(),
                            };

            if (parentName != null)
            {
                var parent = _repository.Get(parentName);
                if (parent != null)
                {
                    model.ParentId = parent.Id;
                    model.ParentName = parent.PageName;
                    if (parent.ChildTemplate != null)
                    {
                        model.TemplateId = parent.ChildTemplate.Id;
                        model.Content = parent.ChildTemplate.Content;
                    }

                   
                }
            }


            model.Templates = _templateRepository.Find().Select(x => new SelectListItem
                                                                         {
                                                                             Selected = x.Id == model.TemplateId,
                                                                             Text = x.Title,
                                                                             Value =
                                                                                 x.Id.ToString(
                                                                                     CultureInfo.InvariantCulture)
                                                                         });

            return View(model);
        }


        public ActionResult Revisions(string id)
        {
            var page = _repository.Get(id);

            var userIds = page.Revisions.Select(k => k.CreatedBy).ToList();
            if (!userIds.Contains(page.UpdatedBy))
                userIds.Add(page.UpdatedBy);

            var items = new List<DiffViewModelItem>
                                                {
                                                    new DiffViewModelItem
                                                        {
                                                            RevisionId = 0,
                                                            CreatedAt = page.UpdatedAt,
                                                            UserDisplayName = page.UpdatedBy.DisplayName
                                                        }
                                                };
            items.AddRange(page.Revisions.Select(history =>
                                                 new DiffViewModelItem
                                                     {
                                                         RevisionId = history.Id,
                                                         CreatedAt = history.CreatedAt,
                                                         UserDisplayName = page.UpdatedBy.DisplayName
                                                     }));


            return View(new DiffViewModel
                            {
                                PageName = id,
                                Revisions = items
                            });
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Compare(string id, int first, int second)
        {
            var page = _repository.Get(id);

            var diff1 = first == 0
                            ? page.HtmlBody
                            : page.Revisions.First(k => k.Id == first).HtmlBody;
            var diff2 = second == 0
                            ? page.HtmlBody
                            : page.Revisions.First(k => k.Id == second).HtmlBody;

            var differ = new HtmlDiff(diff1, diff2);
            return Json(new
                            {
                                success = true,
                                content = differ.Build()
                            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, Transactional2, Authorize]
        public ActionResult Create(CreateViewModel model)
        {
            //if (!ModelState.IsValid)
            //    return View(model);

            /*try
            {*/
                var page =_pageService.CreatePage(model.ParentId, model.PageName, model.Title, model.Content,0);
                
                return RedirectToAction("Show", new { id = model.PageName });
            /*}
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                Logger.Error("Failed to create page " + model.PageName, ex);
            }

            return View(model);*/
        }

        [Authorize]
        public ActionResult Delete(string id)
        {
            var page = _repository.Get(id);
            return View(new DeleteViewModel
                            {
                                PageName = page.PageName,
                                Title = page.Title,
                                Children = page.Children.Select(x => x.PageName).ToList()
                            });
        }

        [HttpPost, Transactional2, Authorize]
        public ActionResult Delete(DeleteViewModel model)
        {
            _pageService.DeletePage(model.PageName);
            return RedirectToAction("Index");
        }

    }

    public class Transactional2Attribute : ActionFilterAttribute
    {
        private IUnitOfWork _uow;
        private ILogger _logger = LogManager.GetLogger<Transactional2Attribute>();

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _uow = DependencyResolver.Current.GetService<IUnitOfWork>();
            if (_uow == null)
                throw new ConfigurationErrorsException(
                    "Failed to find an IUnitOfWork implementation in the IoC container.");

            if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                filterContext.Result = new ViewResult
                {
                    ViewData = filterContext.Controller.ViewData,
                    ViewName = filterContext.ActionDescriptor.ActionName,
                    TempData = filterContext.Controller.TempData
                };
            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// Called by the ASP.NET MVC framework after the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                filterContext.Controller.ViewData.ModelState.AddModelError("", filterContext.Exception.Message);
                _logger.Error("Failed to store changes in the repository " + filterContext.ActionDescriptor.ActionName, filterContext.Exception);
                filterContext.Result = new ViewResult
                                           {
                                               ViewData = filterContext.Controller.ViewData,
                                               ViewName = filterContext.ActionDescriptor.ActionName,
                                               TempData = filterContext.Controller.TempData
                                           };
            }
            else if (filterContext.Controller.ViewData.ModelState.IsValid && filterContext.Exception == null)
            {
                _uow.SaveChanges();
            }

            // no need to dispose. let the container handle that

            base.OnActionExecuted(filterContext);
        }
    }
}