using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Griffin.Logging;
using Griffin.Wiki.Core;
using Griffin.Wiki.Core.Authorization;
using Griffin.Wiki.Core.Infrastructure.Authorization.Mvc;
using Griffin.Wiki.Core.Pages;
using Griffin.Wiki.Core.Pages.Content.Services;
using Griffin.Wiki.Core.Pages.Repositories;
using Griffin.Wiki.Core.Pages.Services;
using Griffin.Wiki.Core.SiteMaps.Repositories;
using Griffin.Wiki.Core.Templates.Repositories;
using Griffin.Wiki.Mvc3.Areas.Wiki.Models.Page;
using Griffin.Wiki.Mvc3.Helpers;
using Helpers;
using Sogeti.Pattern.Data;

namespace Griffin.Wiki.Mvc3.Areas.Wiki.Controllers
{
    [WikiAuthorize]
    public class PageController : BaseController
    {
        private readonly IAuthorizer _authorizer;
        private readonly PageService _pageService;
        private readonly IPageTreeRepository _pageTreeRepository;
        private readonly IPageRepository _repository;
        private readonly ITemplateRepository _templateRepository;

        public PageController(IPageRepository repository, PageService pageService,
                              ITemplateRepository templateRepository, IPageTreeRepository pageTreeRepository,
                              IAuthorizer authorizer)
        {
            _repository = repository;
            _pageService = pageService;
            _templateRepository = templateRepository;
            _pageTreeRepository = pageTreeRepository;
            _authorizer = authorizer;
        }

        public ActionResult Index()
        {
            return Show(new WikiRoot(), "Home");
        }

        public ActionResult Show(PagePath pagePath, string id)
        {
            var loadContext = _pageService.Load(pagePath);
            if (loadContext == null)
            {
                return this.RedirectToWikiPage(pagePath);
            }

            var tocBuilder = new TableOfContentsBuilder();
            tocBuilder.Compile(loadContext.Body);

            var tree = _pageTreeRepository.Get(loadContext.Page.Id);
            var model = new ShowViewModel
                            {
                                Body = loadContext.Body,
                                PagePath = pagePath,
                                Title = loadContext.Page.Title,
                                UpdatedAt = loadContext.Page.UpdatedAt,
                                UserName = loadContext.Page.UpdatedBy.DisplayName,
                                BackLinks = loadContext.Page.BackReferences.ToList(),
                                TableOfContents = tocBuilder.GenerateList(),
                                Path = tree.CreateLinksForPath(Url.WikiRoot())
                            };

            return View("Show", model);
        }

        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult QuickSearch(string term)
        {
            var items = _repository.FindTop10(term);
            return
                Json(items.Select(x => new
                                           {
                                               link = Url.WikiPage(x.PagePath),
                                               title = x.Title,
                                               description = x.CreateAbstract()
                                           }), JsonRequestBehavior.AllowGet);
        }


        public ActionResult Edit(PagePath id)
        {
            var path = id ?? new WikiRoot();
            var page = _repository.Get(path);
            var revision = page.GetLatestRevision();

            var model = new EditViewModel {Path = path, Title = page.Title, Content = revision.RawBody};
            return View(model);
        }

        [HttpPost, Transactional2]
        public ActionResult Edit(EditViewModel model)
        {
            _pageService.UpdatePage(model.Path, model.Title, model.Content, model.Comment);

            return !User.IsInRole(WikiRole.User)
                       ? RedirectToAction("ReviewRequired", new {id = model.Path})
                       : this.RedirectToWikiPage(model.Path);
        }

        public ActionResult ReviewRequired(PagePath id)
        {
            return View(id);
        }

        public ActionResult Create(PagePath id, string title = null)
        {
            if (!User.IsInRole(WikiRole.Contributor))
                return View("MayNotCreate");

            var model = new CreateViewModel
                            {
                                PagePath = id,
                                Title = title ?? id.Name,
                                Content = "",
                                Templates = new List<SelectListItem>(),
                            };

            var parent = _repository.Get(id.ParentPath);
            if (parent != null)
            {
                model.ParentId = parent.Id;
                model.ParentPath = parent.PagePath.ToString();
                if (parent.ChildTemplate != null)
                {
                    model.TemplateId = parent.ChildTemplate.Id;
                    model.Content = parent.ChildTemplate.Content;
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
            if (string.IsNullOrEmpty(id))
                id = "/";
            else if (!id.StartsWith("/"))
                id = "/" + id;
            var path = new PagePath(id);
            var page = _repository.Get(path);

            var userIds = page.Revisions.Select(k => k.CreatedBy).ToList();
            if (!userIds.Contains(page.UpdatedBy))
                userIds.Add(page.UpdatedBy);

            var items = page.Revisions.Select(history =>
                                              new DiffViewModelItem
                                                  {
                                                      RevisionId = history.Id,
                                                      CreatedAt = history.CreatedAt,
                                                      UserDisplayName = page.UpdatedBy.DisplayName,
                                                      Comment = history.ChangeDescription
                                                  }).OrderByDescending(x => x.CreatedAt).ToList();


            return View(new DiffViewModel
                            {
                                Path = path,
                                Revisions = items
                            });
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Compare(string path, int first, int second)
        {
            var page = _repository.Get(new PagePath(path));

            var id1 = Math.Min(first, second);
            var id2 = Math.Max(first, second);

            var diff1 = page.Revisions.First(k => k.Id == id1).HtmlBody;
            var diff2 = page.Revisions.First(k => k.Id == id2).HtmlBody;

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
            var page = _pageService.CreatePage(model.ParentId, model.PagePath, model.Title, model.Content, 0);

            if (_authorizer.CanCreateBelow(page) || _authorizer.CanCreatePages())
            {
                return View("Created", page);
            }

            return this.RedirectToWikiPage(page);
        }

        [Authorize]
        public ActionResult Delete(PagePath id)
        {
            var page = _repository.Get(id);
            return View(new DeleteViewModel
                            {
                                PageName = page.PagePath.ToString(),
                                Title = page.Title,
                                Children = page.Children.Select(x => x.PagePath.ToString()).ToList()
                            });
        }

        [HttpPost, Transactional2, Authorize]
        public ActionResult Delete(DeleteViewModel model)
        {
            var path = new PagePath(model.PageName);
            _pageService.DeletePage(path);
            return this.RedirectToWikiPage(new PagePath("/"));
        }
    }

    public class Transactional2Attribute : ActionFilterAttribute
    {
        private readonly ILogger _logger = LogManager.GetLogger<Transactional2Attribute>();
        private IUnitOfWork _uow;

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
                _logger.Error("Failed to store changes in the repository " + filterContext.ActionDescriptor.ActionName,
                              filterContext.Exception);
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