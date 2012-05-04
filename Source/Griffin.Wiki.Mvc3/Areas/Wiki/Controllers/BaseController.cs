using System.Web.Mvc;
using Griffin.Logging;

namespace Griffin.Wiki.Mvc3.Areas.Wiki.Controllers
{
    [ErrorFilter]
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

        protected override void OnException(ExceptionContext filterContext)
        {
            string info = "\r\nRouteData Values:\r\n";
            foreach (var data in filterContext.RouteData.Values)
            {
                info += string.Format("{0}: {1}\r\n", data.Key, data.Value);
            }

            info += "\r\nRouteData DataTokens:\r\n";
            foreach (var data in filterContext.RouteData.DataTokens)
            {
                info += string.Format("{0}: {1}\r\n", data.Key, data.Value);
            }

            info += "\r\nQuerystring:\r\n";
            foreach (string key in filterContext.RequestContext.HttpContext.Request.QueryString)
            {
                info += string.Format("{0}: {1}\r\n", key, filterContext.RequestContext.HttpContext.Request.QueryString[key]);
            }

            info += "\r\nForm:\r\n";
            foreach (string key in filterContext.RequestContext.HttpContext.Request.Form)
            {
                info += string.Format("{0}: {1}\r\n", key, filterContext.RequestContext.HttpContext.Request.Form[key]);
            }

            Logger.Error("Unhandled exception." + info, filterContext.Exception);
            base.OnException(filterContext);
        }

        private bool ViewExists(string name)
        {
            ViewEngineResult result = ViewEngines.Engines.FindView(ControllerContext, name, null);
            return (result.View != null);
        }

        protected ActionResult ViewOrPartial(object model)
        {
            if (Request.IsAjaxRequest())
                return PartialView(model);

            return View(model);
        }
    }
}
