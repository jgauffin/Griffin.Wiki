using System;

namespace ProjectPortal.Core.DomainModels
{
    /// <summary>
    /// A user in the system.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        protected User()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="accountName">Name of the account.</param>
        public User(string displayName, string accountName)
        {
            if (displayName == null) throw new ArgumentNullException("displayName");
            if (accountName == null) throw new ArgumentNullException("accountName");

            DisplayName = displayName;
            AccountName = accountName;
        }

        /// <summary>
        /// Gets user identity.
        /// </summary>
        public virtual int Id { get; protected set; }

        /// <summary>
        /// Gets account name
        /// </summary>
        public virtual string AccountName { get; protected set; }

        /// <summary>
        /// Gets full name
        /// </summary>
        public virtual string DisplayName { get; protected set; }
    }
}