using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Griffin.Logging;

namespace Griffin.Wiki.WebClient.Controllers
{
    public class BaseController : Controller
    {
        private readonly ILogger _logger;

        protected BaseController()
        {
            _logger = LogManager.GetLogger(GetType());
        }

        protected ILogger Logger
        {
            get { return _logger; }
        }

        protected ActionResult ViewOrPartial()
        {
            var currentAction = ControllerContext.RouteData.Values["action"].ToString();
            if (Request.IsAjaxRequest())
            {
                if (ViewExists("_" + currentAction))
                    return PartialView("_" + currentAction);

                return PartialView(currentAction);
            }

            return View(currentAction);
        }

        private bool ViewExists(string name)
        {
            ViewEngineResult result = ViewEngines.Engines.FindView(ControllerContext, name, null);
            return (result.View != null);
        }
    }
}
