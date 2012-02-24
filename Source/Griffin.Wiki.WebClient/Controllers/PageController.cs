using System;
using System.Web;
using System.Web.Mvc;
using Griffin.Wiki.Core.Repositories;
using Griffin.Wiki.Core.Services;
using Griffin.Wiki.WebClient.Models.Page;

namespace Griffin.Wiki.WebClient.Controllers
{
    public class PageController : BaseController
    {
        private readonly IPageRepository _repository;
        private readonly PageService _pageService;

        public PageController(IPageRepository repository, PageService pageService)
        {
            _repository = repository;
            _pageService = pageService;
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
                throw new HttpException(404, "Page " + id + " do not exist.");

            return View(page);
        }

        public ActionResult Create(string pageName, string title = null)
        {
            var model = new CreateViewModel
                            {
                                PageName = pageName,
                                Title = title ?? pageName,
                                Content = ""
                            };

            return View(model);
        }


        [HttpPost]
        public ActionResult Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                _pageService.CreatePage(model.PageName, model.Title, model.Content);
                return RedirectToAction("Show", new { id = model.PageName });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                Logger.Error("Failed to create page " + model.PageName, ex);
            }

            return View(model);
        }

    }
}