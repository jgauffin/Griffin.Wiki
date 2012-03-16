using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Griffin.Wiki.WebClient.Areas.Wiki
{
    /// <summary>
    /// Loads the <see cref="IWikiAuthorizationFilter"/> through <see cref="DependencyResolver"/> to validate users.
    /// </summary>
    /// <returns>Used by all wiki controllers to do authorization</returns>
    public class WikiAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        class AuthContext : IWikiAuthorizationContext
        {
            public AuthorizationContext AuthorizationContext { get; set; }

            public string PageName
            {
                get;
                set;
            }

            public string PagePath
            {
                get;
                set;
            }
        }
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var authorizer = DependencyResolver.Current.GetService<IWikiAuthorizationFilter>();
            if (authorizer != null)
            {
                var ctx = new AuthContext
                              {
                                  AuthorizationContext = filterContext,
                                  PageName = filterContext.RouteData.Values["pageName"].ToString(),
                                  PagePath = filterContext.RouteData.Values["wikiPath"].ToString()
                              };

                authorizer.Authorize(ctx);
            }
        }
       
    }
}