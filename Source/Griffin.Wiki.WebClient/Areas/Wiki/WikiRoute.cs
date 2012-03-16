using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Griffin.Logging;
using Griffin.Wiki.Core.Repositories;
using Griffin.Wiki.Core.Repositories.Mappings;

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
            : base(string.Format("{0}/{{*wikiPath}}", prefix), new RouteValueDictionary(defaults), new MvcRouteHandler())
        {
            _prefix = prefix.ToLower();
            DataTokens = new RouteValueDictionary();
            DataTokens["area"] = _prefix;
            DataTokens["UseNamespaceFallback"] = false;
            DataTokens["Namespaces"] = new string[] {GetType().Namespace + ".*"};
        }


        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            string virtualPath = httpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(2) + (httpContext.Request.PathInfo ?? string.Empty);

            if (!virtualPath.ToLower().StartsWith(_prefix))
                return null;

            var route2 = base.GetRouteData(httpContext);
            if (route2 == null)
                return null;

            var wikiPath = route2.Values["wikiPath"] == null ? "Home/" : route2.Values["wikiPath"].ToString();

            //if (route2.Values["wikiPath"] == null)
            //var path = route2.Values["wikiPath"].ToString();

            var routeData = new RouteData(this, RouteHandler);
            /*foreach (KeyValuePair<string, object> value in values)
            {
                routeData.Values.Add(value.Key, value.Value);
            }*/
            if (this.DataTokens != null)
            {
                foreach (KeyValuePair<string, object> token in this.DataTokens)
                {
                    routeData.DataTokens.Add(token.Key, token.Value);
                }
            }

            var repos = DependencyResolver.Current.GetService<PageTreeRepository>();
            routeData.Values["controller"] = "Page";
            routeData.Values["action"] = "Show";


            if (wikiPath == "/")
                wikiPath = "Home/";

            if (!wikiPath.EndsWith("/"))
                wikiPath += "/";

            if (wikiPath.Count(x => x == '/') == 1) // "pageName/"
            {
                routeData.Values["pageName"] = wikiPath.Trim('/');
                var node = repos.GetByName(wikiPath.Trim('/'));
                if (node == null)
                {
                    routeData.Values["action"] = "Create";
                    routeData.Values["id"] = wikiPath.Trim('/');
                }
            }
            else
            {
                var node = repos.GetByPath(wikiPath);
                if (node != null)
                {
                    routeData.Values["pageName"] = node.Page.PageName;
                }

                else
                {
                    var name = wikiPath.Trim('/');
                    var pos = name.LastIndexOf('/');
                    var parentName = pos == -1 ? "Home" : name.Substring(pos + 1);
                    if (parentName == "")
                        parentName = "Home";

                    //var parent = repos.GetByName(parentName);
                    routeData.Values["parentName"] = parentName;
                    routeData.Values["action"] = "Create";
                    routeData.Values["pageName"] = name;
                    routeData.Values["id"] = name;
                }
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
            var result2 = base.GetVirtualPath(requestContext, values);
            var repos = DependencyResolver.Current.GetService<PageTreeRepository>();
            if (values.Count == 0)
                return base.GetVirtualPath(requestContext, values);

            if (values["wikiRoot"] != null)
            {
                 var path= new VirtualPathData(this, _prefix);
                foreach (var kvp in DataTokens)
                {
                    path.DataTokens.Add(kvp.Key, kvp.Value);
                }
                return path;
            }

            var pageName = values["pageName"] == null ? null : values["pageName"].ToString();
            if (pageName != null)
            {
                if (pageName == "Home")
                    return new VirtualPathData(this, _prefix);

                var page = repos.GetByName(pageName);
                if (page == null)
                    return null;

                values["wikiPath"] = page.Names;
                var result3 =  base.GetVirtualPath(requestContext, values);
                //return result3;

                var relative = string.Format("{0}{1}", _prefix, page.Names);
                var path = new VirtualPathData(this, relative);
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