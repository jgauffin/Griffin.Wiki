using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Griffin.Wiki.Core.Repositories;

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


        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Routing.Route"/> class, by using the specified URL pattern, default parameter values, and handler class. 
        /// </summary>
        /// <param name="prefix">Prefix that all wiki page urls must start with. "wiki" is suggested</param>
        /// <param name="defaults">Specify controller and action used to handle the pages.</param>
        public WikiRoute(string prefix, object defaults)
            : base(string.Format("{0}/{{wikiPath*}}", prefix), new RouteValueDictionary(defaults), new RouteValueDictionary { { "area", prefix } }, new MvcRouteHandler())
        {
            _prefix = prefix;
        }


        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            if (!httpContext.Request.Url.AbsolutePath.StartsWith("/" + _prefix))
                return null;

            var route2 = base.GetRouteData(httpContext);

            var routeData = new RouteData(this, RouteHandler);

            var repos = DependencyResolver.Current.GetService<PageTreeRepository>();


            var wikiPath = "/" + routeData.Values["wikiPath"];
            if (wikiPath == "/")
            {
                routeData.Values["pageName"] = "Home";
                routeData.Values["controller"] = "Page";
                routeData.Values["action"] = "Show";
                return routeData;
            }

            if (!wikiPath.EndsWith("/"))
                wikiPath += "/";

            if (wikiPath.Count(x => x == '/') == 2) // "/pageName/"
            {
                var node = repos.GetByName(wikiPath.Trim('/'));
                if (node != null)
                {
                    routeData.Values["pageName"] = node.Page.PageName;
                    return routeData;
                }
            }
            else
            {
                var node = repos.GetByPath(wikiPath);
                if (node != null)
                {
                    routeData.Values["pageName"] = node.Page.PageName;
                    return routeData;
                }
            }
            
            return null;
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
            var result2 = base.GetVirtualPath(requestContext, values);
            var repos = DependencyResolver.Current.GetService<PageTreeRepository>();
            if (values.Count == 0)
                return base.GetVirtualPath(requestContext, values);

            if (values["wikiRoot"] != null)
            {
                 var path= new VirtualPathData(this, "/wiki/");
                foreach (var kvp in values)
                {
                    path.DataTokens.Add(kvp.Key, kvp.Value);
                }
                return path;
            }

            var pageName = values["pageName"] == null ? null : values["pageName"].ToString();
            if (pageName != null)
            {
                var page = repos.GetByName(pageName);
                if (page == null)
                    return null;

                values["wikiPath"] = page.Names;
                var result3 =  base.GetVirtualPath(requestContext, values);
                //return result3;
                var relative = string.Format("{0}{1}", _prefix, page.Names);
                var path= new VirtualPathData(this, relative);
                foreach (var kvp in values)
                {
                    path.DataTokens.Add(kvp.Key, kvp.Value);
                }
                return path;
            }

            var wikiPath = values["wikiPath"] == null ? null : values["wikiPath"].ToString();
            if (wikiPath != null)
            {
                //values["wikiPath"] = page.Names;
                //var relative = string.Format("~/{0}/{1}", _prefix, wikiPath);
                //return new VirtualPathData(this, relative);
            }

            var result = base.GetVirtualPath(requestContext, values);
            return result;
        }

        public string Area
        {
            get { return _prefix; }
        }
    }
}