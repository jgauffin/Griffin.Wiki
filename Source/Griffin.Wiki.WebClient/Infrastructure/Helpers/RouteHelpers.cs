using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Griffin.Wiki.Core.DomainModels;
using Griffin.Wiki.Core.Pages.DomainModels;
using Griffin.Wiki.Core.Repositories;

namespace Griffin.Wiki.WebClient.Infrastructure.Helpers
{
    public static class RouteHelpers
    {
        public static string WikiPage(this UrlHelper helper, string pageName)
        {
            return helper.RouteUrl("Wiki", new {pageName});
        }

        public static MvcHtmlString WikiPageLink(this HtmlHelper helper, string title, string pageName, object htmlAttributes = null)
        {
            var attributes = new RouteValueDictionary(htmlAttributes);
            var repos = DependencyResolver.Current.GetService<IPageRepository>();
            var page = repos.Get(pageName);
            if (page == null)
            {
                attributes["class"] += "missing";
                return helper.ActionLink(title, "Create", "Page", new{title=title}, attributes);
            }

            return helper.RouteLink(title, "Wiki", new { pageName = page.PageName }, htmlAttributes);
        }

        public static MvcHtmlString WikiPageLink(this HtmlHelper helper, string pageName, object htmlAttributes = null)
        {
            var attributes = new RouteValueDictionary(htmlAttributes);
            var repos = DependencyResolver.Current.GetService<IPageRepository>();
            var page = repos.Get(pageName);
            if (page == null)
            {
                attributes["class"] += "missing";
                return helper.ActionLink(pageName, "Create", "Page", null, attributes);
            }

            return helper.RouteLink(page.Title, "Wiki", new { pageName = page.PageName }, htmlAttributes);
        }

        public static ActionResult RedirectToWikiPage(this Controller controller, string pageName)
        {
            var repos = DependencyResolver.Current.GetService<IPageRepository>();
            var page = repos.Get(pageName);
            if (page != null)
            {
                return RedirectToWikiPage(controller, page);
            }

            return new RedirectResult(controller.Url.Action("Create", "Page", new {pageName}));
        }

        public static string WikiRoot(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("Wiki", new{ wikiRoot = "wikiRoot"});
        }

        public static ActionResult RedirectToWikiPage(this Controller controller, WikiPage page)
        {
            var url = controller.Url.RouteUrl("Wiki", new {pageName = page.PageName, wikiPage = page});
            return new RedirectResult(url);
        }
    }
}