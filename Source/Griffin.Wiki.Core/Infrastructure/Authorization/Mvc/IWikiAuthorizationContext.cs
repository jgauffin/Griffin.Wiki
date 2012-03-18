using System.Web.Mvc;

namespace Griffin.Wiki.Core.Infrastructure.Authorization.Mvc
{
    /// <summary>
    /// Content used to authenticate wiki requests.
    /// </summary>
    public interface IWikiAuthorizationContext
    {
        /// <summary>
        /// Gets ASP.NET MVC3 authentication context.
        /// </summary>
        AuthorizationContext AuthorizationContext { get; }

        /// <summary>
        /// Gets page that the user wants to work with
        /// </summary>
        string PageName { get; }

        /// <summary>
        /// Get path to the page.
        /// </summary>
        string PagePath { get; }
    }
}