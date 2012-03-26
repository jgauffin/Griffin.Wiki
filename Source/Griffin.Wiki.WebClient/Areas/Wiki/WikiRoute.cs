using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Griffin.Logging;
using Griffin.Wiki.Core.Pages;
using Griffin.Wiki.Core.SiteMaps.Repositories;

namespace Griffin.Wiki.WebClient.Areas.Wiki
{
    public static class RouteCollectionExtension
    {
        public static void MapWikiRoute(this RouteCollection collection, string prefix, object defaults)
        {
            collection.Add("Wiki", new WikiRoute(prefix, defaults));
        }
    }

    public class WikiRoute : Route, IRouteWithArea
    {
        private readonly string _prefix;
        private ILogger _logger = LogManager.GetLogger<WikiRoute>();


        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Routing.Route"/> class, by using the specified URL pattern, default parameter values, and handler class. 
        /// </summary>
        /// <param name="prefix">Prefix that all wiki page urls must start with. "wiki" is suggested</param>
        /// <param name="defaults">Specify controller and action used to handle the pages.</param>
        public WikiRoute(string prefix, object defaults)
            : base(string.Format("{0}/{{*pagePath}}", prefix), new RouteValueDictionary(defaults), new MvcRouteHandler())
        {
            _prefix = prefix.ToLower();
            DataTokens = new RouteValueDictionary();
            DataTokens["area"] = _prefix;
            DataTokens["UseNamespaceFallback"] = false;
            DataTokens["Namespaces"] = new string[] { GetType().Namespace + ".*" };
        }


        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            string virtualPath = httpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(2) + (httpContext.Request.PathInfo ?? string.Empty);
            if (!virtualPath.ToLower().StartsWith(_prefix))
                return null;
            if (virtualPath.ToLower().StartsWith(_prefix + "/adm/"))
                return null;

            var route2 = base.GetRouteData(httpContext);
            if (route2 == null)
                return null;

            var pagePath = route2.Values["pagePath"] == null ? "/" : route2.Values["pagePath"].ToString();
            if (!pagePath.StartsWith("/"))
                pagePath = "/" + pagePath;

            var routeData = new RouteData(this, RouteHandler);
            if (this.DataTokens != null)
            {
                foreach (KeyValuePair<string, object> token in this.DataTokens)
                {
                    routeData.DataTokens.Add(token.Key, token.Value);
                }
            }

            var repos = DependencyResolver.Current.GetService<IPageTreeRepository>();
            routeData.Values["controller"] = "Page";
            routeData.Values["action"] = "Show";

            if (!pagePath.EndsWith("/"))
                pagePath += "/";


            var node = repos.GetByPath(pagePath);
            if (node != null)
            {
                routeData.Values["pagePath"] = node.Page.PagePath;
            }
            else
            {
                var name = pagePath.Trim('/');
                var pos = name.LastIndexOf('/');
                var parentName = pos == -1 ? "/" : name.Substring(pos + 1);
                if (parentName == "")
                    parentName = "/";

                //var parent = repos.GetByPath(parentName);
                routeData.Values["parentName"] = parentName;
                routeData.Values["action"] = "Create";
                routeData.Values["pageName"] = name;
                routeData.Values["id"] = name;
            }


            return routeData;
        }

        /// <summary>
        /// Returns information about the URL that is associated with the route.
        /// </summary>
        /// <param name="requestContext">An object that encapsulates information about the requested route.</param>
        /// <param name="values">An object that contains the parameters for a route.</param>
        /// <returns>
        /// An object that contains information about the URL that is associated with the route.
        /// </returns>
        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            var repos = DependencyResolver.Current.GetService<IPageTreeRepository>();
            if (values.Count == 0)
                return base.GetVirtualPath(requestContext, values);

            if (values["wikiRoot"] != null)
            {
                var virtualPath = new VirtualPathData(this, _prefix);
                foreach (var kvp in DataTokens)
                {
                    virtualPath.DataTokens.Add(kvp.Key, kvp.Value);
                }
                return virtualPath;
            }

            var path = values["pagePath"] == null ? null : values["pagePath"].ToString();
            if (path != null)
            {
                if (path == "/")
                    return new VirtualPathData(this, _prefix);

                var page = repos.GetByPath(new PagePath(path));
                if (page == null)
                    return null;

                values["pagePath"] = page.Path;
                var relative = string.Format("{0}{1}", _prefix, page.Path);
                var virtualPath = new VirtualPathData(this, relative);
                foreach (var kvp in values)
                {
                    virtualPath.DataTokens.Add(kvp.Key, kvp.Value);
                }

                return virtualPath;
            }

            var result = base.GetVirtualPath(requestContext, values);
            if (result == null)
                return null;

            return result;
        }

        public string Area
        {
            get { return _prefix; }
        }
    }
}