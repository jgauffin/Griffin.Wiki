using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Griffin.Wiki.Core.DomainModels;
using NHibernate;
using NHibernate.Linq;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.Repositories
{
    [Component]
    public class FakeRepository : IUserRepository
    {
        private readonly ISession _session;

        public FakeRepository(ISession session)
        {
            _session = session;
        }

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

        public User GetOrCreate(string accountName)
        {
            var user = _session.Query<User>().FirstOrDefault(x => x.AccountName == accountName);
            if (user == null)
            {
                user = new User(Thread.CurrentPrincipal.Identity.Name, Thread.CurrentPrincipal.Identity.Name);
                _session.Save(user);
                _session.Flush();
            }

            return user;
        }
    }
}
