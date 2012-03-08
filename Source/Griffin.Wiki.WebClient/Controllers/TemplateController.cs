using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Griffin.Wiki.WebClient.Controllers
{
    public class TemplateController : Controller
    {
        //
        // GET: /Template/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return PartialView();
        }

    }
}
