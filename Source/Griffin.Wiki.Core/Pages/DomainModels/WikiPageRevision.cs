using System;
using System.Security.Permissions;
using Griffin.Wiki.Core.Infrastructure;
using Griffin.Wiki.Core.Pages.Content.Services;
using Griffin.Wiki.Core.Pages.DomainModels.Events;
using Griffin.Wiki.Core.Pages.PreProcessors;
using Griffin.Wiki.Core.Users.DomainModels;
using Sogeti.Pattern.DomainEvents;

namespace Griffin.Wiki.Core.Pages.DomainModels
{
    /// <summary>
    /// A revision of an wiki page.
    /// </summary>
    public class WikiPageRevision
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WikiPageRevision"/> class.
        /// </summary>
        /// <param name="source">The source page (before modifications).</param>
        /// <param name="comment">Comment describing the changes.</param>
        public WikiPageRevision(WikiPage source, string comment)
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

        /// <summary>
        /// Initializes a new instance of the <see cref="WikiPageRevision"/> class.
        /// </summary>
        /// <param name="page">Page that the revision is for.</param>
        /// <param name="parserResult">Parsed body</param>
        /// <param name="comment">Comment describing the changes.</param>
        /// <param name="createdBy">Person that created the revision</param>
        public WikiPageRevision(WikiPage page, User createdBy, PreProcessorContext parserResult, string comment)
        {
            if (page == null) throw new ArgumentNullException("source");
            if (createdBy == null) throw new ArgumentNullException("createdBy");
            if (parserResult == null) throw new ArgumentNullException("parserResult");
            if (comment == null) throw new ArgumentNullException("comment");

            ChangeDescription = comment;
            CreatedAt = DateTime.Now;
            CreatedBy = createdBy;
            HtmlBody = parserResult.Body;
            RawBody = parserResult.OriginalBody;
            Page = page;
        }

        protected WikiPageRevision()
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

        /// <summary>
        /// Gets if a review is required
        /// </summary>
        public virtual bool ReviewRequired { get; set; }

        /// <summary>
        /// Gets person that reviewed this revision
        /// </summary>
        public virtual User ReviewedBy { get; protected set; }

        /// <summary>
        /// Gets if the edit has been approvied (if a review was required)
        /// </summary>
        public virtual bool? IsApproved { get; protected set; }

        /// <summary>
        /// Gets reason to why approval was denied (during a review)
        /// </summary>
        public virtual string Reason { get; protected set; }

        /// <summary>
        /// Gets when a review was done
        /// </summary>
        public virtual DateTime? ReviewedAt { get; protected set; }

        /// <summary>
        /// Approve revision (if review was required)
        /// </summary>
        //[PrincipalPermission(SecurityAction.Demand, Role = WikiRole.Contributor)]
        public virtual void Approve()
        {
            if (!ReviewRequired)
                throw new InvalidOperationException("A review is not required. Edit cannot be approved.");

            ReviewedBy = WikiContext.CurrentUser;
            ReviewedAt = DateTime.Now;
            IsApproved = true;
            DomainEventDispatcher.Current.Dispatch(new EditApproved(this));
        }

        /// <summary>
        /// Approve revision (if review was required)
        /// </summary>
        /// <remarks>The revision was OK, but will be improved with additional information. i.e. approve it but
        /// don't publish the revision</remarks>
        //[PrincipalPermission(SecurityAction.Demand, Role = WikiRole.Contributor)]
        public virtual void ApproveButWillImprove()
        {
            if (!ReviewRequired)
                throw new InvalidOperationException("A review is not required. Edit cannot be approved.");

            ReviewedBy = WikiContext.CurrentUser;
            ReviewedAt = DateTime.Now;
            IsApproved = true;
        }

        /// <summary>
        /// Deny the edit
        /// </summary>
        /// <param name="reason">Why the edit was denied</param>
        //[PrincipalPermission(SecurityAction.Demand, Role = WikiRole.Contributor)]
        public virtual void Deny(string reason)
        {
            if (reason == null) throw new ArgumentNullException("reason");
            if (!ReviewRequired)
                throw new InvalidOperationException("A review is not required. Edit cannot be approved.");

            ReviewedBy = WikiContext.CurrentUser;
            ReviewedAt = DateTime.Now;
            IsApproved = false;
            Reason = reason;

            DomainEventDispatcher.Current.Dispatch(new EditDenied(this, reason));
        }
    }
}