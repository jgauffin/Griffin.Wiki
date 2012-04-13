using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Griffin.Wiki.Core.Pages.Repositories;

namespace Griffin.Wiki.Mvc3.Areas.Wiki.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IPageRepository _pageRepository;

        public DocumentController(IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }

        public ActionResult Index()
        {
            var all = _pageRepository.FindAll();


            return View();
        }

        //private void Load()


    }
}
