using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Griffin.Wiki.Core.DomainModels;

namespace Griffin.Wiki.Core.Infrastructure
{
    public class WikiPrinicpal : IPrincipal
    {
        private readonly WikiIdentity _identity;

        public WikiPrinicpal(WikiIdentity identity)
        {
            _identity = identity;
        }

        public bool IsInRole(string role)
        {
            return true;
        }

        public IIdentity Identity
        {
            get { return _identity; }
        }
    }

    public class WikiIdentity : IIdentity, ICurrentUser
    {
        private readonly User _user;

        public WikiIdentity(User user)
        {
            _user = user;
        }

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

        public int UserId
        {
            get { return _user.Id; }
        }
    }
}
