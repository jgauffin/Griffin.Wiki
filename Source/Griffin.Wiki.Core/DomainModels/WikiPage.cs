using System;
using System.Collections.Generic;
using System.Linq;
using Griffin.Wiki.Core.Repositories;
using Griffin.Wiki.Core.Services;

namespace Griffin.Wiki.Core.DomainModels
{
    /// <summary>
    ///   A page in the wiki
    /// </summary>
    public class WikiPage
    {
        private readonly IPageRepository _repository;

        /// <summary>
        ///   Protected to help nhibernate.
        /// </summary>
        protected List<WikiPageLink> _backLinks = new List<WikiPageLink>();

        /// <summary>
        ///   Protected to help nhibernate
        /// </summary>
        protected List<WikiPageHistory> _history = new List<WikiPageHistory>();

        /// <summary>
        ///   Protected to help nhibernate
        /// </summary>
        protected List<WikiPageLink> _links = new List<WikiPageLink>();

        /// <summary>
        ///   Initializes a new instance of the <see cref="WikiPage" /> class.
        /// </summary>
        public WikiPage(IPageRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");

            _repository = repository;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        ///   Gets database id.
        /// </summary>
        public virtual int Id { get; protected set; }

        /// <summary>
        ///   Gets all pages that references the current one.
        /// </summary>
        public virtual IEnumerable<WikiPageLink> BackReferences
        {
            get { return _backLinks; }
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
        ///   Gets all revisions of the page.
        /// </summary>
        public virtual IEnumerable<WikiPageHistory> History
        {
            get { return _history; }
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
        public virtual IEnumerable<WikiPageLink> References
        {
            get { return _links; }
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
        /// <param name="rawBody"> </param>
        /// <param name="result"> </param>
        public void SetBody(string rawBody, IWikiParserResult result)
        {
            if (rawBody == null) throw new ArgumentNullException("rawBody");
            if (result == null) throw new ArgumentNullException("result");

            RawBody = rawBody;
            AddBackLinks(result.PageLinks);
            HtmlBody = result.Content;

            var newLinks = result.PageLinks;
            var removedLinks = _links.Select(k => k.LinkedPage.PageName).Except(newLinks);
            RemoveBackLinks(removedLinks);
        }

        private void AddBackLinks(IEnumerable<string> pageNames)
        {
            foreach (var pageLink in pageNames)
            {
                var page = _repository.Get(pageLink);
                if (page != null)
                    page.AddReferer(this);
            }
        }

        private void RemoveBackLinks(IEnumerable<string> pageNames)
        {
            foreach (var pageLink in pageNames)
            {
                var page = _repository.Get(pageLink);
                if (page != null)
                    page.RemoveReferer(this);
            }
        }

        public void RemoveReferer(WikiPage referer)
        {
            if (referer == null) throw new ArgumentNullException("referer");

            _backLinks.RemoveAll(k => k.LinkedPage.PageName == referer.PageName);
        }

        /// <summary>
        ///   Add another page that references this one.
        /// </summary>
        /// <param name="referer"> Page that links to the current </param>
        public void AddReferer(WikiPage referer)
        {
            if (referer == null) throw new ArgumentNullException("referer");

            if (_backLinks.Any(k => k.LinkedPage.PageName == referer.PageName))
                return;

            _backLinks.Add(new WikiPageLink(referer, this));
        }
    }
}