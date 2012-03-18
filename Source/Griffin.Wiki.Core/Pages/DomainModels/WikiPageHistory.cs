using System;
using Griffin.Wiki.Core.Users.DomainModels;

namespace Griffin.Wiki.Core.Pages.DomainModels
{
    /// <summary>
    /// A revision of an wiki page.
    /// </summary>
    public class WikiPageHistory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WikiPageHistory"/> class.
        /// </summary>
        /// <param name="source">The source page (before modifications).</param>
        /// <param name="comment">Comment describing the changes.</param>
        public WikiPageHistory(WikiPage source, string comment)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (comment == null) throw new ArgumentNullException("comment");

            ChangeDescription = comment;
            CreatedAt = source.UpdatedAt;
            CreatedBy = source.UpdatedBy;
            HtmlBody = source.HtmlBody;
            RawBody = source.RawBody;
            Page = source;
        }

        protected WikiPageHistory()
        {
        }

        /// <summary>
        /// Gets or sets database id.
        /// </summary>
        public virtual int Id { get; protected set; }

        /// <summary>
        /// Gets or sets description of the revision
        /// </summary>
        public virtual string ChangeDescription { get; set; }

        /// <summary>
        /// Gets or sets when the revision was created
        /// </summary>
        public virtual DateTime CreatedAt { get; protected set; }

        /// <summary>
        /// Gets generated HTML body
        /// </summary>
        public virtual string HtmlBody { get; protected set; }

        /// <summary>
        /// Gets body as typed by user
        /// </summary>
        public virtual string RawBody { get; protected set; }

        /// <summary>
        /// Gets user that created the revision
        /// </summary>
        public virtual User CreatedBy { get; protected set; }

        /// <summary>
        /// Gets or sets page that this is an revision for.
        /// </summary>
        public virtual WikiPage Page { get; protected set; }
    }
}