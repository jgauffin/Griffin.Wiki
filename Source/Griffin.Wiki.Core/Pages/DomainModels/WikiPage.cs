using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using Griffin.Container.DomainEvents;
using Griffin.Wiki.Core.Infrastructure;
using Griffin.Wiki.Core.Pages.Content.Services;
using Griffin.Wiki.Core.Pages.DomainModels.Events;
using Griffin.Wiki.Core.Pages.PreProcessors;
using Griffin.Wiki.Core.Pages.Repositories;
using Griffin.Wiki.Core.Templates.DomainModels;
using Griffin.Wiki.Core.Users.DomainModels;

using Griffin.Container;

namespace Griffin.Wiki.Core.Pages.DomainModels
{
    /// <summary>
    ///   A page in the wiki
    /// </summary>
    public class WikiPage
    {
        private readonly IList<WikiPage> _children = new List<WikiPage>();
        private readonly IList<WikiPage> _references = new List<WikiPage>();
        private readonly IList<WikiPageRevision> _revisions = new List<WikiPageRevision>();
        private string _pagePath;

        /// <summary>
        ///   Initializes a new instance of the <see cref="WikiPage" /> class.
        /// </summary>
        /// <param name="parent"> Parent page. </param>
        /// <param name="pagePath"> Absolute path from wiki root. </param>
        /// <param name="title"> Page title. </param>
        /// <param name="template"> Template to use for child pages (if any). </param>
        public WikiPage(WikiPage parent, PagePath pagePath, string title, PageTemplate template)
        {
            if (pagePath == null) throw new ArgumentNullException("pagePath");
            if (title == null) throw new ArgumentNullException("title");

            Parent = parent;
            PagePath = pagePath;
            Title = title;
            CreatedBy = WikiContext.Current.User;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            ChildTemplate = template;
            _backReferences = new List<WikiPage>();
        }

        protected WikiPage()
        {
        }

        /// <summary>
        ///   Gets database id.
        /// </summary>
        public virtual int Id { get; protected set; }

        /// <summary>
        ///   Gets id of the active revision
        /// </summary>
        protected virtual int ActiveRevisionId { get; set; }

        /// <summary>
        ///   Gets all pages that references the current one.
        /// </summary>
        public virtual IEnumerable<WikiPage> BackReferences
        {
            get { return _backReferences; }
        }

        /// <summary>
        ///   Gets or sets template for all child pages
        /// </summary>
        /// <remarks>
        ///   Use <c>Parent.ChildTemplate</c> to get the template for this page.
        /// </remarks>
        public virtual PageTemplate ChildTemplate { get; set; }

        /// <summary>
        ///   Gets all pages child pages.
        /// </summary>
        public virtual IEnumerable<WikiPage> Children
        {
            get { return _children; }
        }

        /// <summary>
        ///   Gets when the page as created
        /// </summary>
        public virtual DateTime CreatedAt { get; protected set; }

        /// <summary>
        ///   Gets user that created the page
        /// </summary>
        public virtual User CreatedBy { get; protected set; }

        /// <summary>
        ///   Gets generated HTML body (after parsing the Raw body)
        /// </summary>
        public virtual string HtmlBody { get; protected set; }

        /// <summary>
        ///   Gets absolute path (from wiki root) to the current page
        /// </summary>
        public virtual PagePath PagePath
        {
            get { return new PagePath(_pagePath); }
            set { _pagePath = value.ToString(); }
        }

        /// <summary>
        ///   Gets parent page.
        /// </summary>
        public virtual WikiPage Parent { get; protected set; }

        /// <summary>
        ///   Gets body as the user typed it.
        /// </summary>
        public virtual string RawBody { get; protected set; }

        /// <summary>
        ///   Gets all pages that the current one references.
        /// </summary>
        public virtual IEnumerable<WikiPage> References
        {
            get { return _references; }
        }

        /// <summary>
        ///   Gets all revisions of the page.
        /// </summary>
        public virtual IEnumerable<WikiPageRevision> Revisions
        {
            get { return _revisions; }
        }

        /// <summary>
        ///   Gets a friendly title
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        ///   Gets when the last update was made.
        /// </summary>
        public virtual DateTime UpdatedAt { get; protected set; }

        /// <summary>
        ///   Gets user who did the last update
        /// </summary>
        public virtual User UpdatedBy { get; protected set; }

        protected virtual IList<WikiPage> _backReferences { get; set; }

        /// <summary>
        ///   Get currently used revision.
        /// </summary>
        /// <returns> </returns>
        public virtual WikiPageRevision GetLatestRevision()
        {
            return _revisions.Where(x=>x.IsApproved).OrderByDescending(x => x.CreatedAt).First();
        }


        /// <summary>
        ///   Set the body information
        /// </summary>
        /// <param name="result"> Parsed body and found links </param>
        /// <param name="comment"> </param>
        /// <param name="repository"> Used to updat page relations </param>
        public virtual void SetBody(PreProcessorContext result, string comment, IPageRepository repository)
        {
            if (result == null) throw new ArgumentNullException("result");

            if (Thread.CurrentPrincipal.IsInRole(WikiRole.User))
            {
                AddRevision(result, comment, repository);
            }
            else
            {
                AddPendingRevision(result, comment, repository);
            }

            UpdateLinksInternal(result, repository);
        }

        private void AddRevision(PreProcessorContext result, string comment, IPageRepository repository)
        {
            var isNew = !_revisions.Any();


            UpdatedAt = DateTime.Now;
            UpdatedBy = WikiContext.Current.User;
            RawBody = result.OriginalBody;
            HtmlBody = result.Body;
            repository.Save(this);

            if (isNew)
            {
                DomainEvent.Publish(new PageCreated(this));
            }
            else
            {
                DomainEvent.Publish(new PageUpdated(this));
            }

            var revision = new WikiPageRevision(this, comment);
            revision.Approve();
            repository.Save(revision);
            _revisions.Add(revision);
        }

        private void AddPendingRevision(PreProcessorContext result, string comment, IPageRepository repository)
        {
            var revision = new WikiPageRevision(this, WikiContext.Current.User, result, comment) {ReviewRequired = true};
            repository.Save(revision);
            _revisions.Add(revision);
            DomainEvent.Publish(new RevisionModerationRequired(revision));
        }

        /// <summary>
        ///   Move page to another parent
        /// </summary>
        /// <param name="newParent"> New parent page </param>
        public virtual void Move(WikiPage newParent)
        {
            if (newParent == null) throw new ArgumentNullException("newParent");

            var oldParent = Parent;
            Parent = newParent;
            DomainEvent.Publish(new PageMoved(this, oldParent));
        }

      
        /// <summary>
        ///   The body have been reparsed to reflect changed links.
        /// </summary>
        public virtual void UpdateLinks(PreProcessorContext result, IPageRepository repository)
        {
            //var result = Parser.Parse(PageName, RawBody);
            HtmlBody = result.Body;
            UpdateLinksInternal(result, repository);
        }

        private void UpdateLinksInternal(PreProcessorContext result, IPageRepository repository)
        {
            var added = result.LinkedPages.Except(References.Select(k => k.PagePath)).ToList();
            if (added.Any())
            {
                var pages = repository.GetPages(added);
                foreach (var page in pages)
                {
                    page.BackReferences.Any(); // lazy load.
                    page._backReferences.Add(this);
                }


                var missingPages = added.Except(pages.Select(x => x.PagePath));
                repository.AddMissingLinks(this, missingPages);
            }


            var removed = References.Select(k => k.PagePath).Except(result.LinkedPages).ToList();
            if (removed.Any())
                RemoveBackLinks(removed);
        }

        private void RemoveBackLinks(IEnumerable<PagePath> removedPageLinks)
        {
            var removedPages = (from p in References
                                where removedPageLinks.Contains(p.PagePath)
                                select p);
            foreach (var removedPage in removedPages)
            {
                removedPage._backReferences.Remove(this);
            }
        }

        /// <summary>
        ///   Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj"> The <see cref="System.Object" /> to compare with this instance. </param>
        /// <returns> <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c> . </returns>
        public override bool Equals(object obj)
        {
            var other = obj as WikiPage;
            return other != null && Id.Equals(other.Id);
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns> A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            return ("WikiPage" + Id).GetHashCode();
        }

        /// <summary>
        ///   Set the body to a specific revision
        /// </summary>
        /// <param name="repository"> Used to parse links </param>
        /// <param name="revision"> Revision ot use </param>
        /// <param name="result"> Result from body parsing </param>
        public virtual void SetRevision(IPageRepository repository, WikiPageRevision revision, PreProcessorContext result)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            if (revision == null) throw new ArgumentNullException("revision");
            if (result == null) throw new ArgumentNullException("result");

            UpdatedAt = revision.CreatedAt;
            UpdatedBy = revision.CreatedBy;
            RawBody = result.OriginalBody;
            HtmlBody = result.Body;
            DomainEvent.Publish(new PageUpdated(this));
            UpdateLinksInternal(result, repository);
        }

        /// <summary>
        /// Creates an abstract of the article
        /// </summary>
        /// <returns></returns>
        /// <returns>The first paragraph of each page is considered to be an abstract. This method returns
        /// the paragraph (max 255 chars)</returns>
        public virtual string CreateAbstract()
        {
            var pos = RawBody.IndexOf("\r\n\r\n", System.StringComparison.Ordinal);
            if (pos == -1)
            {
                return Title;
            }

            var msg = RawBody.Substring(0, pos);
            if (msg.Length > 255)
                return msg.Substring(0, 250) + "[...]";

            return msg;
        }
    }
}