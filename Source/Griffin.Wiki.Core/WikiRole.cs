using Griffin.Wiki.Core.Authorization;
using Griffin.Wiki.Core.Infrastructure.Authorization;

namespace Griffin.Wiki.Core
{
    /// <summary>
    /// Roles used by <see cref="RoleBasedAuthorizer"/>
    /// </summary>
    public static class WikiRole
    {
        /// <summary>
        /// May browse the wiki and edit pages (but all edits must be approved)
        /// </summary>
        public const string Viewer = "WikiViewer";

        /// <summary>
        /// May edit existing pages (without edits being reviewed)
        /// </summary>
        public const string User = "WikiUser";

        /// <summary>
        /// May create new pages and templates
        /// </summary>
        public const string Contributor = "WikiContributor";

        /// <summary>
        /// Can perform all administrative takss.
        /// </summary>
        public const string Administrator = "WikiAdministrator";
    }
}