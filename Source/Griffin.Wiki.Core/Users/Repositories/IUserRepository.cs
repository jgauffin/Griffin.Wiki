using System.Collections.Generic;
using Griffin.Wiki.Core.Users.DomainModels;

namespace Griffin.Wiki.Core.Users.Repositories
{
    /// <summary>
    /// Used to work with users.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Get display names for the specified users
        /// </summary>
        /// <param name="userIds">All requested users.</param>
        /// <returns>UserId, DisplayName</returns>
        IDictionary<int, string> GetDisplayNames(IEnumerable<int> userIds);

        /// <summary>
        /// Get or create a user
        /// </summary>
        /// <param name="accountName">Account name</param>
        /// <param name="displayName">Display name</param>
        /// <returns>Created user</returns>
        User GetOrCreate(string accountName, string displayName);
    }
}