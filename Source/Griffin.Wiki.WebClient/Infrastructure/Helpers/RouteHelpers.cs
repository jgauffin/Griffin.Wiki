using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Griffin.Wiki.Core.DomainModels;
using Griffin.Wiki.Core.Pages;
using Griffin.Wiki.Core.Pages.DomainModels;
using Griffin.Wiki.Core.Pages.Repositories;

namespace Griffin.Wiki.WebClient.Infrastructure.Helpers
{
    public static class RouteHelpers
    {
        public static string WikiPage(this UrlHelper helper, PagePath pagePath)
        {
            return helper.RouteUrl("Wiki", new {pagePath = pagePath});
        }

        public static MvcHtmlString WikiPageLink(this HtmlHelper helper, string title, PagePath pagePath, object htmlAttributes = null)
        {
            var attributes = new RouteValueDictionary(htmlAttributes);
            var repos = DependencyResolver.Current.GetService<IPageRepository>();
            var page = repos.Get(pagePath);
            if (page == null)
            {
                attributes["class"] += "missing";
                return helper.ActionLink(title, "Create", "Page", new{title=title}, attributes);
            }

            return helper.RouteLink(title, "Wiki", new { pagePath = page.PagePath }, htmlAttributes);
        }

        public static MvcHtmlString WikiPageLink(this HtmlHelper helper, PagePath pagePath, object htmlAttributes = null)
        {
            var attributes = new RouteValueDictionary(htmlAttributes);
            var repos = DependencyResolver.Current.GetService<IPageRepository>();
            var page = repos.Get(pagePath);
            if (page == null)
            {
                attributes["class"] += "missing";
                return helper.ActionLink(pagePath.ToString(), "Create", "Page", null, attributes);
            }

            return helper.RouteLink(page.Title, "Wiki", new { pagePath = page.PagePath }, htmlAttributes);
        }

        public static ActionResult RedirectToWikiPage(this Controller controller, PagePath pagePath)
        {
            var repos = DependencyResolver.Current.GetService<IPageRepository>();
            var page = repos.Get(pagePath);
            if (page != null)
            {
                return RedirectToWikiPage(controller, page);
            }

            return new RedirectResult(controller.Url.Action("Create", "Page", new { pagePath }));
        }

        public static string WikiRoot(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("Wiki", new{ wikiRoot = "wikiRoot"});
        }

        public static ActionResult RedirectToWikiPage(this Controller controller, WikiPage page)
        {
            var url = controller.Url.RouteUrl("Wiki", new { pagePath = page.PagePath, wikiPage = page });
            return new RedirectResult(url);
        }
    }
}