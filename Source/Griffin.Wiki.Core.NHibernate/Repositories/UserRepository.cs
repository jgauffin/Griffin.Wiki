using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Griffin.Wiki.Core.Users.DomainModels;
using Griffin.Wiki.Core.Users.Repositories;
using NHibernate;
using NHibernate.Linq;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.NHibernate.Repositories
{
    [Component]
    public class UserRepository : IUserRepository
    {
        private readonly ISession _session;

        public UserRepository(ISession session)
        {
            _session = session;
        }

        #region IUserRepository Members

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

        #endregion

        public string GetDisplayName(int userId)
        {
            return "Jonas";
        }
    }
}