namespace Griffin.Wiki.Core.Infrastructure.Authorization.Mvc
{
    /// <summary>
    /// Used to authorize users in the wiki
    /// </summary>
    /// <remarks>The wiki do not have a built in system used to handle authorization.
    /// It's instead you that should take care of that.
    /// </remarks>
    public interface IWikiAuthorizationFilter
    {
        /// <summary>
        /// Authorize the page.
        /// </summary>
        /// <param name="authorizationContext">Authorization context</param>
        void Authorize(IWikiAuthorizationContext authorizationContext);
    }
}