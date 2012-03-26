using Griffin.Wiki.Core.Authorization;

namespace Griffin.Wiki.Core.Infrastructure.Authorization.Mvc
{
    /// <summary>
    /// Uses roles to authenticate pages.
    /// </summary>
    public class RoleBasedAuthorizationFilter : IWikiAuthorizationFilter
    {
        private readonly RoleBasedAuthorizer _roleBasedAuthorizer;

        public RoleBasedAuthorizationFilter(RoleBasedAuthorizer roleBasedAuthorizer)
        {
            _roleBasedAuthorizer = roleBasedAuthorizer;
        }

        #region IWikiAuthorizationFilter Members

        /// <summary>
        /// Authorize the page.
        /// </summary>
        /// <param name="authorizationContext">Authorization context</param>
        public void Authorize(IWikiAuthorizationContext authorizationContext)
        {
            if (authorizationContext.PagePath == null)
            {
                
            }
        }

        #endregion
    }
}