using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectPortal.Core.Repositories;

namespace ProjectPortal.Controllers
{
    public class PageController : Controller
    {
        private readonly IPageRepository _repository;

        public PageController(IPageRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            return View();
        }

        protected override void HandleUnknownAction(string actionName)
        {
            base.HandleUnknownAction(actionName);
        }

        public ActionResult Details(string id)
        {
            var page = _repository.Get(id);
            if (page == null)
                throw new HttpException(404, "Page " + id + " do not exist.");

            return View(page);
        }

    }
}
