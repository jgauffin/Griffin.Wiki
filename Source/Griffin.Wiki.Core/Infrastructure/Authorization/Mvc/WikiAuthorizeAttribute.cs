using System.Web.Mvc;
using Griffin.Wiki.Core.Pages;

namespace Griffin.Wiki.Core.Infrastructure.Authorization.Mvc
{
    /// <summary>
    /// Loads the <see cref="IWikiAuthorizationFilter"/> through <see cref="DependencyResolver"/> to validate users.
    /// </summary>
    /// <returns>Used by all wiki controllers to do authorization</returns>
    public class WikiAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        #region IAuthorizationFilter Members

        /// <summary>
        /// Called when authorization is required.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var pagePathValue = filterContext.RouteData.Values["pagePath"];
            if (pagePathValue == null)
                return;

            var authorizer = DependencyResolver.Current.GetService<IWikiAuthorizationFilter>();

            if (authorizer != null)
            {
                var ctx = new AuthContext
                              {
                                  AuthorizationContext = filterContext,
                                  PagePath = new PagePath(pagePathValue.ToString())
                              };

                authorizer.Authorize(ctx);
            }
        }

        #endregion

        #region Nested type: AuthContext

        private class AuthContext : IWikiAuthorizationContext
        {
            #region IWikiAuthorizationContext Members

            public AuthorizationContext AuthorizationContext { get; set; }

            public PagePath PagePath { get; set; }

            #endregion
        }

        #endregion
    }
}