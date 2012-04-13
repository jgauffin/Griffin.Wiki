using System;
using System.Threading;
using Griffin.Wiki.Core.Users.DomainModels;

namespace Griffin.Wiki.Core.Infrastructure
{
    /// <summary>
    /// Used on the current <code>IIdentity</code> implementation to provide the wiki user.
    /// </summary>
    public interface ICurrentUser
    {
        /// <summary>
        /// Get current wiki user.
        /// </summary>
        User WikiUser { get; }
    }

    /// <summary>
    /// Used to identity the current user.
    /// </summary>
    /// <returns>The data source implementations may want to store the user in data source. It's therefore important
    /// that you use </returns>
    public class WikiContext
    {
        private static WikiContext _current = new WikiContext();

        /// <summary>
        /// Gets or sets the current implementation
        /// </summary>
        public static WikiContext Current
        {
            get
            {
                return _current;
            }
            set { _current = value; }
        }

        /// <summary>
        /// Get current user.
        /// </summary>
        public virtual User User
        {
            get
            {
                var usr = Thread.CurrentPrincipal.Identity as ICurrentUser;
                if (usr == null)
                    throw new InvalidOperationException(
                        "Either let your Thread.CurrentPrincipal.Identity class implement Griffin.Wiki.Core.Infrastructure.ICurrentUser or assign a new WikiContext which" +
                        "provides the current user.");

                return usr.WikiUser;
            }
        }
    }
}