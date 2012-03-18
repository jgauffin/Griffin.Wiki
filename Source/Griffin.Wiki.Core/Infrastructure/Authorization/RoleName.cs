namespace Griffin.Wiki.Core.Authorization
{
    /// <summary>
    /// Roles used by <see cref="RoleBasedAuthorizer"/>
    /// </summary>
    public static class RoleName
    {
        /// <summary>
        /// May only browse the wiki
        /// </summary>
        public const string Viewer = "WikiViewer";

        /// <summary>
        /// May edit existing pages.
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