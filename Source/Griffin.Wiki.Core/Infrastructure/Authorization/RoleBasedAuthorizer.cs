using System.Threading;
using Griffin.Wiki.Core.Authorization;
using Griffin.Wiki.Core.Pages.DomainModels;
using Griffin.Container;

namespace Griffin.Wiki.Core.Infrastructure.Authorization
{
    /// <summary>
    /// Uses roles to handle authorization
    /// </summary>
    /// <remarks>
    /// 
    /// <para>
    /// Expects the following roles to exist:
    /// </para>
    /// <list type="table">
    /// <listheader>
    /// <term>Role name</term>
    /// <description>What the role is used for</description>
    /// </listheader>
    /// <item>
    /// <term>WikiViewer</term>
    /// <description>May look at all pages.</description>
    /// </item>
    /// <item>
    /// <term>WikiUser</term>
    /// <description>May edit existing pages.</description>
    /// </item>
    /// <item>
    /// <term>WikiContributor</term>
    /// <description>May create new pages and templates</description>
    /// </item>
    /// <item>
    /// <term>WikiAdministrator</term>
    /// <description>Can move and delete pages, create/edit templates.</description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <seealso cref="WikiRole"/>
    [Component]
    public class RoleBasedAuthorizer : IAuthorizer
    {
        #region IAuthorizer Members

        /// <summary>
        /// Can edit a page
        /// </summary>
        /// <param name="page">Page to edit</param>
        /// <returns>true if user can edit the specified page; otherwise false.</returns>
        public bool CanEdit(WikiPage page)
        {
            return Thread.CurrentPrincipal.IsInRole(WikiRole.User)
                   || Thread.CurrentPrincipal.IsInRole(WikiRole.Contributor)
                   || Thread.CurrentPrincipal.IsInRole(WikiRole.Administrator);
        }

        /// <summary>
        /// Delete privileges
        /// </summary>
        /// <param name="page">Current page</param>
        /// <returns>true if user can delete the specified page; otherwise false.</returns>
        public bool CanDelete(WikiPage page)
        {
            return Thread.CurrentPrincipal.IsInRole(WikiRole.Administrator);
        }

        /// <summary>
        /// View privileges
        /// </summary>
        /// <param name="page">Current page</param>
        /// <returns>true if user can view the specified page; otherwise false.</returns>
        public bool CanView(WikiPage page)
        {
            return Thread.CurrentPrincipal.IsInRole(WikiRole.Viewer) || CanEdit(page);
        }

        /// <summary>
        /// Can create pages at all
        /// </summary>
        /// <returns>true if user can perform the operation; otherwise false.</returns>
        public bool CanCreatePages()
        {
            return Thread.CurrentPrincipal.IsInRole(WikiRole.Contributor);
        }

        /// <summary>
        /// Can create child pages for the specified on
        /// </summary>
        /// <param name="page">Page</param>
        /// <returns>true if user can perform the operation; otherwise false.</returns>
        public bool CanCreateBelow(WikiPage page)
        {
            return Thread.CurrentPrincipal.IsInRole(WikiRole.Contributor);
        }

        #endregion
    }
}