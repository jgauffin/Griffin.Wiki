using Griffin.Wiki.Core.Pages.DomainModels;

namespace Griffin.Wiki.Core.Authorization
{
    /// <summary>
    /// Used to check if the currently logged in user can perform the specified options
    /// </summary>
    public interface IAuthorizer
    {
        /// <summary>
        /// Can edit a page
        /// </summary>
        /// <param name="page">Page to edit</param>
        /// <returns>true if user can edit the specified page; otherwise false.</returns>
        bool CanEdit(WikiPage page);

        /// <summary>
        /// Delete privileges
        /// </summary>
        /// <param name="page">Current page</param>
        /// <returns>true if user can delete the specified page; otherwise false.</returns>
        bool CanDelete(WikiPage page);

        /// <summary>
        /// View privileges
        /// </summary>
        /// <param name="page">Current page</param>
        /// <returns>true if user can view the specified page; otherwise false.</returns>
        bool CanView(WikiPage page);

        /// <summary>
        /// Can create pages at all
        /// </summary>
        /// <returns>true if user can perform the operation; otherwise false.</returns>
        bool CanCreatePages();

        /// <summary>
        /// Can create child pages for the specified on
        /// </summary>
        /// <param name="page">Page</param>
        /// <returns>true if user can perform the operation; otherwise false.</returns>
        bool CanCreateBelow(WikiPage page);
    }
}