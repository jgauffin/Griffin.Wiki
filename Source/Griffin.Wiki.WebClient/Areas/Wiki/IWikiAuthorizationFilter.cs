using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Griffin.Wiki.Core.DomainModels;
using Griffin.Wiki.Core.Repositories.Mappings;

namespace Griffin.Wiki.WebClient.Areas.Wiki
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

    public interface IWikiAuthorizationContext
    {
        AuthorizationContext AuthorizationContext { get; }
        string PageName { get; }
        string PagePath { get; }
    }
}