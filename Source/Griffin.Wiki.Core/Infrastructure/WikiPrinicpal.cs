using System.Security.Principal;
using Griffin.Wiki.Core.Users.DomainModels;

namespace Griffin.Wiki.Core.Infrastructure
{
    public class WikiPrinicpal : IPrincipal
    {
        private readonly WikiIdentity _identity;

        public WikiPrinicpal(WikiIdentity identity)
        {
            _identity = identity;
        }

        #region IPrincipal Members

        public bool IsInRole(string role)
        {
            return true;
        }

        public IIdentity Identity
        {
            get { return _identity; }
        }

        #endregion
    }

    public class WikiIdentity : IIdentity, ICurrentUser
    {
        private readonly User _user;

        public WikiIdentity(User user)
        {
            _user = user;
        }

        #region ICurrentUser Members

        public User WikiUser
        {
            get { return _user; }
        }

        #endregion

        #region IIdentity Members

        public string Name
        {
            get { return _user.DisplayName; }
        }

        public string AuthenticationType
        {
            get { return "SQL"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        #endregion
    }
}