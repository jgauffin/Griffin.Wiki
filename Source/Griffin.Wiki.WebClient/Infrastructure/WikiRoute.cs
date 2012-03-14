using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Griffin.Wiki.Core.Repositories;

namespace Griffin.Wiki.WebClient.Infrastructure
{
    public static class RouteCollectionExtension
    {
        public static void MapWikiRoute(this RouteCollection collection, string prefix, object defaults)
        {
            collection.Add("Wiki", new WikiRoute(prefix, defaults));
        }
    }

    public class WikiRoute : Route
    {
        private readonly string _prefix;


        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Routing.Route"/> class, by using the specified URL pattern, default parameter values, and handler class. 
        /// </summary>
        /// <param name="prefix">Prefix that all wiki page urls must start with. "wiki" is suggested</param>
        /// <param name="defaults">Specify controller and action used to handle the pages.</param>
        public WikiRoute(string prefix, object defaults)
            : base(string.Format("{0}/{{*wikiPath}}", prefix), new RouteValueDictionary(defaults), new MvcRouteHandler())
        {
            _prefix = prefix;
        }


        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var routeData = base.GetRouteData(httpContext);
            if (routeData == null)
                return null;

            var repos = DependencyResolver.Current.GetService<PageTreeRepository>();


            var wikiPath = "/" + routeData.Values["wikiPath"];
            if (wikiPath == "/")
            {
                routeData.Values["pageName"] = "Home";
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
            
            var repos = DependencyResolver.Current.GetService<PageTreeRepository>();

            if (requestContext.RouteData.Values["pageName"] != null && values.Count == 0)
                return new VirtualPathData(this, VirtualPathUtility.ToAbsolute("~/" + _prefix));

            var pageName = values["pageName"] == null ? null : values["pageName"].ToString();
            if (pageName != null)
            {
                var page = repos.GetByName(pageName);
                if (page == null)
                    return null;

                var relative = string.Format("~/{0}/{1}", _prefix, pageName);
                return new VirtualPathData(this, relative);
            }

            var wikiPath = values["wikiPath"] == null ? null : values["wikiPath"].ToString();
            if (wikiPath != null)
            {
                var relative = string.Format("~/{0}/{1}", _prefix, wikiPath);
                return new VirtualPathData(this, relative);
            }

            var result = base.GetVirtualPath(requestContext, values);
            return result;
        }
    }
}