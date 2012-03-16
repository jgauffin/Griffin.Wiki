using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Griffin.Logging;
using Griffin.Wiki.Core.Repositories;
using Griffin.Wiki.Core.Services;
using Griffin.Wiki.WebClient.Areas.Wiki.Models.Page;
using Griffin.Wiki.WebClient.Controllers;
using Griffin.Wiki.WebClient.Infrastructure.Helpers;
using Griffin.Wiki.WebClient.Areas.Wiki.Models.Page;
using Helpers;
using Sogeti.Pattern.Data;

namespace Griffin.Wiki.WebClient.Areas.Wiki.Controllers
{
    [WikiAuthorize]
    public class PageController : BaseController
    {
        private readonly IPageRepository _repository;
        private readonly PageService _pageService;
        private readonly TemplateRepository _templateRepository;
        private readonly PageTreeRepository _pageTreeRepository;

        public PageController(IPageRepository repository, PageService pageService, TemplateRepository templateRepository, PageTreeRepository pageTreeRepository)
        {
            _repository = repository;
            _pageService = pageService;
            _templateRepository = templateRepository;
            _pageTreeRepository = pageTreeRepository;
        }

        public ActionResult Index()
        {
            return Show("Home", "Home");
        }

        public ActionResult Show(string pageName, string id)
        {
            if (id != null && pageName == null)
                pageName = id;

            var page = _repository.Get(pageName);
            if (page == null)
            {
                return this.RedirectToWikiPage(pageName);
            }

            var tocBuilder = new TableOfContentsBuilder();
            tocBuilder.Compile(page.HtmlBody);

            var tree = _pageTreeRepository.Get(page.Id);

            var model = new ShowViewModel
                            {
                                Body = page.HtmlBody,
                                PageName = pageName,
                                Title = page.Title,
                                UpdatedAt = page.UpdatedAt,
                                UserName = page.UpdatedBy.DisplayName,
                                BackLinks = page.BackReferences.Select(k => k.PageName).ToList(),
                                TableOfContents = tocBuilder.GenerateList(),
                                Path = tree.CreateLinkPath(Url.WikiRoot())
                            };

            return View("Show", model);
        }

        public ActionResult Edit(string id)
        {
            var page = _repository.Get(id);
            var model = new EditViewModel { PageName = id, Title = page.Title, Content = page.RawBody };
            return View(model);
        }

        [HttpPost, Transactional2]
        public ActionResult Edit(EditViewModel model)
        {
            _pageService.UpdatePage(model.PageName, model.Title, model.Content, model.Comment);

            return this.RedirectToWikiPage(model.PageName);
        }

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

            var differ = new HtmlDiff(diff2, diff1);
            return Json(new
                            {
                                success = true,
                                content = differ.Build()
                            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, Transactional2, Authorize]
        public ActionResult Create(CreateViewModel model)
        {
            var page = _pageService.CreatePage(model.ParentId, model.PageName, model.Title, model.Content,0);
            return this.RedirectToWikiPage(page);
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
            return this.RedirectToWikiPage("Home");
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