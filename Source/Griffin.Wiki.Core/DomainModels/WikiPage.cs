using System;
using System.Collections.Generic;
using System.Linq;
using Griffin.Wiki.Core.Infrastructure;
using Griffin.Wiki.Core.Repositories;
using Griffin.Wiki.Core.Services;
using Sogeti.Pattern.DomainEvents;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.DomainModels
{
    /// <summary>
    ///   A page in the wiki
    /// </summary>
    [Component]
    public class WikiPage
    {
        private readonly IList<WikiPage> _backReferences = new List<WikiPage>();
        private readonly IList<WikiPage> _children = new List<WikiPage>();
        private readonly IList<WikiPage> _references = new List<WikiPage>();
        private readonly IList<WikiPageHistory> _revisions = new List<WikiPageHistory>();

        /// <summary>
        ///   Initializes a new instance of the <see cref="WikiPage" /> class.
        /// </summary>
        public WikiPage(string pageName, string title)
        {
            PageName = pageName;
            Title = title;
            CreatedBy = WikiContext.CurrentUser;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        protected WikiPage()
        {
        }

        /// <summary>
        ///   Gets database id.
        /// </summary>
        public virtual int Id { get; protected set; }

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
        ///   Gets wiki name of the page.
        /// </summary>
        public virtual string PageName { get; protected set; }

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
        public virtual IEnumerable<WikiPageHistory> Revisions
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

        /// <summary>
        ///   Set the body information
        /// </summary>
        /// <param name="result">Parsed body and found links </param>
        /// <param name="repository">Used to updat page relations </param>
        public virtual void SetBody(IWikiParserResult result, IPageRepository repository)
        {
            if (result == null) throw new ArgumentNullException("result");

            // Only save history for updated items.
            if (!string.IsNullOrEmpty(RawBody))
                CreateHistoryEntry(repository);

            UpdatedAt = DateTime.Now;
            UpdatedBy = WikiContext.CurrentUser;
            RawBody = result.OriginalBody;
            HtmlBody = result.HtmlBody;
            repository.Save(this);
            if (_revisions.Count > 0)
                repository.Save(_revisions.Last());

            DomainEventDispatcher.Current.Dispatch(new PageUpdated(this));
            UpdateLinksInternal(result, repository);
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
            DomainEventDispatcher.Current.Dispatch(new PageMoved(this, oldParent));
        }

        private void CreateHistoryEntry(IPageRepository repository)
        {
            var history = new WikiPageHistory(this, "");
            _revisions.Add(history);
        }

        /// <summary>
        ///   The body have been reparsed to reflect changed links.
        /// </summary>
        public virtual void UpdateLinks(IWikiParserResult result, IPageRepository repository)
        {
            //var result = Parser.Parse(PageName, RawBody);
            HtmlBody = result.HtmlBody;
            UpdateLinksInternal(result, repository);
        }

        private void UpdateLinksInternal(IWikiParserResult result, IPageRepository repository)
        {
            var added = result.PageLinks.Except(References.Select(k => k.PageName)).ToList();
            var pages = repository.GetPages(added);
            foreach (var page in pages)
                page._backReferences.Add(this);

            var missingPages = added.Except(pages.Select(x => x.PageName));
            repository.AddMissingLinks(this, missingPages);


            var removed = References.Select(k => k.PageName).Except(result.PageLinks).ToList();
            RemoveBackLinks(removed);
        }

        private void RemoveBackLinks(IEnumerable<string> removedPageLinks)
        {
            var removedPages = (from p in References
                                where removedPageLinks.Contains(p.PageName)
                                select p);
            foreach (var removedPage in removedPages)
            {
                removedPage._backReferences.Remove(this);
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as WikiPage;
            return other != null && Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return ("WikiPage" + Id).GetHashCode();
        }
    }

    public class BackLinks
    {
        public IEnumerable<string> Added { get; set; }
        public IEnumerable<string> Removed { get; set; }
    }
}