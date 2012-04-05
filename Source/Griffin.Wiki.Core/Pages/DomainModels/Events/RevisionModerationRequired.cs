using System;
using Sogeti.Pattern.DomainEvents;

namespace Griffin.Wiki.Core.Pages.DomainModels.Events
{
    /// <summary>
    ///   Someone with low privileges have made an edit
    /// </summary>
    /// <returns> The edit must be approved. </returns>
    public class RevisionModerationRequired : IDomainEvent
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="RevisionModerationRequired" /> class.
        /// </summary>
        /// <param name="revision"> The revision to approve. </param>
        public RevisionModerationRequired(WikiPageRevision revision)
        {
            if (revision == null) throw new ArgumentNullException("revision");
            Revision = revision;
        }

        /// <summary>
        ///   Gets revision that must be approved.
        /// </summary>
        public WikiPageRevision Revision { get; private set; }
    }
}