using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Griffin.Wiki.Core.Infrastructure;
using Griffin.Logging;
using System.Web;

namespace Griffin.Wiki.Mvc3
{
    /// <summary>
    /// Custom handler which invokes <see cref="IErrorFilter"/>.
    /// </summary>
    public class ErrorFilterAttribute : FilterAttribute, IExceptionFilter
    {
        private readonly ILogger _logger = LogManager.GetLogger<ErrorFilterAttribute>();

        /// <summary>
        /// Called when an exception occurs.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
                return;

            var filter = DependencyResolver.Current.GetService<IErrorFilter>();
            if (filter != null)
            {
                filterContext.Result = null;
                filter.Handle(filterContext);
                if (filterContext.Result != null)
                    return;
            }

            _logger.Error(BuildContextInformation(filterContext), filterContext.Exception);

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 500;
            var exception = filterContext.Exception as HttpException;
            if (exception != null)
            {
                filterContext.HttpContext.Response.StatusCode = exception.GetHttpCode();
                filterContext.HttpContext.Response.StatusDescription = exception.Message;
            }

            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            filterContext.Result = CreateResult(filterContext, actionName, controllerName);



            // Certain versions of IIS will sometimes use their own error page when 
            // they detect a server error. Setting this property indicates that we
            // want it to try to render ASP.NET MVC's error page instead.
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }

        /// <summary>
        /// Generates a complete string for 
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        protected virtual string BuildContextInformation(ExceptionContext filterContext)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("Failed for the request '{0}'.", filterContext.HttpContext.Request.RawUrl));
            if (filterContext.HttpContext.Request.QueryString.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("QueryString");
                sb.AppendLine("===========");
                foreach (string key in filterContext.HttpContext.Request.QueryString)
                {
                    sb.AppendLine(string.Format("{0}: {1}", key, filterContext.HttpContext.Request.QueryString[key]));
                }
            }
            if (filterContext.HttpContext.Request.Form.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Form");
                sb.AppendLine("===========");
                foreach (string key in filterContext.HttpContext.Request.Form)
                {
                    sb.AppendLine(string.Format("{0}: {1}", key, filterContext.HttpContext.Request.Form[key]));
                }
            }
            if (filterContext.HttpContext.Request.Cookies.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Cookies");
                sb.AppendLine("===========");
                foreach (HttpCookie cookie in filterContext.HttpContext.Request.Cookies)
                {
                    sb.AppendLine(string.Format("{0}: {1}", cookie.Name, cookie.Value));
                }
            }

            return sb.ToString();
        }

        protected virtual string GetViewName(ExceptionContext filterContext)
        {
            var exception = filterContext.Exception as HttpException;
            var statusCode = exception != null ? exception.GetHttpCode() : 500;
            var statusCodeName = ((HttpStatusCode)statusCode).ToString();
            return SelectFirstView(filterContext,
                                              string.Format("~/Views/Error/{0}.cshtml", statusCodeName),
                                              "~/Views/Error/General.cshtml",
                                              statusCodeName,
                                              "Error");
        }


        protected string SelectFirstView(ControllerContext ctx, params string[] viewNames)
        {
            return viewNames.First(view => ViewExists(ctx, view));
        }

        protected bool ViewExists(ControllerContext ctx, string name)
        {
            var result = ViewEngines.Engines.FindView(ctx, name, null);
            return result.View != null;
        }



        protected virtual ActionResult CreateResult(ExceptionContext filterContext, string actionName, string controllerName)
        {
            var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
            return new ViewResult
                                       {
                                           ViewName = GetViewName(filterContext),
                                           ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                                           TempData = filterContext.Controller.TempData
                                       };
        }
    }
}
