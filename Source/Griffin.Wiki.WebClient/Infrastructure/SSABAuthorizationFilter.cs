using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Griffin.Wiki.Core.Infrastructure.Authorization.Mvc;

namespace Griffin.Wiki.WebClient.Infrastructure
{
    public class SSABAuthorizationFilter : IWikiAuthorizationFilter
    {
        /// <summary>
        /// Authorize the page.
        /// </summary>
        /// <param name="authorizationContext">Authorization context</param>
        public void Authorize(IWikiAuthorizationContext authorizationContext)
        {
            
        }
    }
}