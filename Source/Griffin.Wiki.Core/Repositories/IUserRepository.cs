using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Wiki.Core.DomainModels;

namespace Griffin.Wiki.Core.Repositories
{
    public interface IUserRepository
    {
        string GetDisplayName(int userId);

        /// <summary>
        /// Get display names for the specified users
        /// </summary>
        /// <param name="userIds">All requested users.</param>
        /// <returns>UserId, DisplayName</returns>
        IDictionary<int, string> GetDisplayNames(IEnumerable<int> userIds);
    }
}
