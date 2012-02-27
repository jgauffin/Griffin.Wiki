using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.Repositories
{
    [Component]
    public class FakeRepository : IUserRepository
    {
        public string GetDisplayName(int userId)
        {
            return "Jonas";
        }

        /// <summary>
        /// Get display names for the specified users
        /// </summary>
        /// <param name="userIds">All requested users.</param>
        /// <returns>UserId, DisplayName</returns>
        public IDictionary<int, string> GetDisplayNames(IEnumerable<int> userIds)
        {
            var items = new Dictionary<int, string>();
            foreach (var userId in userIds)
            {
                items.Add(userId, "Jonas");
            }

            return items;
        }
    }
}
