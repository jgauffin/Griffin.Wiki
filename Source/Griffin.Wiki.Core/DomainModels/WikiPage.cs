using System;
using System.Collections.Generic;
using System.Linq;
using Griffin.Wiki.Core.Repositories;
using Griffin.Wiki.Core.Services;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.DomainModels
{
    /// <summary>
    ///   A page in the wiki
    /// </summary>
    [Component]
    public class WikiPage
    {
        private IPageRepository _repository;
        IList<WikiPage> _references= new List<WikiPage>();
        IList<WikiPageHistory> _revisions = new List<WikiPageHistory>();
        IList<WikiPage> _backReferences = new List<WikiPage>();

        /// <summary>
        ///   Initializes a new instance of the <see cref="WikiPage" /> class.
        /// </summary>
        public WikiPage(IPageRepository repository, int creator, string pageName, string title)
        {
            if (repository == null) throw new ArgumentNullException("repository");

            _repository = repository;
            PageName = pageName;
            Title = title;
            CreatedBy = creator;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        protected WikiPage()
        {
      
        }

        [InjectMember]
        protected IPageRepository Repository { get { return _repository; } set { _repository = value; } }

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
        ///   Gets when the page as created
        /// </summary>
        public virtual DateTime CreatedAt { get; protected set; }

        /// <summary>
        ///   Gets user that created the page
        /// </summary>
        public virtual int CreatedBy { get; protected set; }

        /// <summary>
        ///   Gets all revisions of the page.
        /// </summary>
        public virtual IEnumerable<WikiPageHistory> Revisions
        {
            get { return _revisions; }
        }

        /// <summary>
        ///   Gets generated HTML body (after parsing the Raw body)
        /// </summary>
        public virtual string HtmlBody { get; protected set; }

        /// <summary>
        ///   Gets wiki name of the page.
        /// </summary>
        public virtual string PageName { get; protected set; }

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
        public virtual int UpdatedBy { get; protected set; }

        /// <summary>
        ///   Set the body information
        /// </summary>
        /// <param name="changer">User that did the current change.</param>
        /// <param name="rawBody"> </param>
        /// <param name="result"> </param>
        public virtual void SetBody(int changer, string rawBody, IWikiParserResult result)
        {
            if (rawBody == null) throw new ArgumentNullException("rawBody");
            if (result == null) throw new ArgumentNullException("result");

            // Only save history for updated items.
            if (!string.IsNullOrEmpty(RawBody))
            {
                var history = new WikiPageHistory(this, "");
                _revisions.Add(history);
                _repository.Save(history);
            }
            
            UpdatedAt = DateTime.Now;
            UpdatedBy = changer;
            RawBody = rawBody;
            HtmlBody = result.Content;

            UpdateLinksInternal(result);
        }

        private void AddBackLinks(IEnumerable<string> pageNames)
        {
            foreach (var pageLink in pageNames)
            {
                var page = _repository.Get(pageLink);
                if (page != null)
                {
                    page._backReferences.Add(this);
                    _references.Add(page);
                }
            }
        }

        private void RemoveBackLinks(IEnumerable<string> pageNames)
        {
            foreach (var pageName in pageNames.ToList())
            {
                var page = _repository.Get(pageName);
                if (page != null)
                {
                    page._backReferences.Remove(this);
                    _references.Remove(page);
                }
            }
        }

        /// <summary>
        /// The body have been reparsed to reflect changed links.
        /// </summary>
        /// <param name="result">Parsed body</param>
        public virtual void UpdateLinks(IWikiParserResult result)
        {
            if (result == null) throw new ArgumentNullException("result");

            HtmlBody = result.Content;
            UpdateLinksInternal(result);
        }

        private void UpdateLinksInternal(IWikiParserResult result)
        {
            var newLinks = result.PageLinks.Except(References.Select(k => k.PageName));
            var removedLinks = References.Select(k => k.PageName).Except(result.PageLinks);
            RemoveBackLinks(removedLinks);
            AddBackLinks(newLinks);
        }

        public override bool Equals(object obj)
        {
            var other = obj as WikiPage;
            if (other == null)
                return false;

            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}