using System;
using System.Collections.Generic;
using System.Configuration;
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

        public PageController(IPageRepository repository, PageService pageService, IUserRepository userRepository)
        {
            _repository = repository;
            _pageService = pageService;
            _userRepository = userRepository;
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

            var model = new ShowViewModel
                            {
                                Body = page.HtmlBody,
                                PageName = id,
                                Title = page.Title,
                                UpdatedAt = page.UpdatedAt,
                                UserName = _userRepository.GetDisplayName(page.UpdatedBy),
                                BackLinks = page.BackReferences.Select(k => k.PageName).ToList()
                            };

            return View(model);
        }

        public ActionResult Edit(string id)
        {
            var page = _repository.Get(id);
            var model = new CreateViewModel {PageName = id, Title = page.Title, Content = page.RawBody};
            return View(model);
        }

        [HttpPost, Transactional2]
        public ActionResult Edit(CreateViewModel model)
        {
            _pageService.UpdatePage(1, model.PageName, model.Title, model.Content);

            return RedirectToAction("Show", new {id = model.PageName});
        }


        public ActionResult Create(string id, string title = null)
        {
            var model = new CreateViewModel
                            {
                                PageName = id,
                                Title = title ?? id,
                                Content = ""
                            };

            return View(model);
        }


        public ActionResult Revisions(string id)
        {
            var page = _repository.Get(id);

            var userIds = page.Revisions.Select(k => k.CreatedBy).ToList();
            if (!userIds.Contains(page.UpdatedBy))
                userIds.Add(page.UpdatedBy);
            var displayNames = _userRepository.GetDisplayNames(userIds);

            var items = new List<DiffViewModelItem>
                                                {
                                                    new DiffViewModelItem
                                                        {
                                                            RevisionId = 0,
                                                            CreatedAt = page.UpdatedAt,
                                                            UserDisplayName = displayNames[page.UpdatedBy]
                                                        }
                                                };
            items.AddRange(page.Revisions.Select(history =>
                                                 new DiffViewModelItem
                                                     {
                                                         RevisionId = history.Id,
                                                         CreatedAt = history.CreatedAt,
                                                         UserDisplayName = displayNames[history.CreatedBy]
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


        [HttpPost, Transactional2]
        public ActionResult Create(CreateViewModel model)
        {
            //if (!ModelState.IsValid)
            //    return View(model);

            /*try
            {*/
                _pageService.CreatePage(1,  model.PageName, model.Title, model.Content);
                return RedirectToAction("Show", new { id = model.PageName });
            /*}
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                Logger.Error("Failed to create page " + model.PageName, ex);
            }

            return View(model);*/
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