using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Griffin.Wiki.Core.Pages;
using Griffin.Wiki.Core.Pages.Content.Services;
using Griffin.Wiki.Core.Pages.DomainModels;
using Griffin.Wiki.Core.Pages.DomainModels.Events;
using Griffin.Wiki.Core.Pages.Repositories;
using Griffin.Wiki.Core.SiteMaps.Repositories;
//using NHibernate;
using Sogeti.Pattern.DomainEvents;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.SiteMaps.Services
{
    /// <summary>
    /// Used to generate and rip down tree.
    /// </summary>
    [Component]
    public class TreeGeneratorService : IAutoSubscriberOf<PageCreated>, IAutoSubscriberOf<PageDeleted>, IPageLinkGenerator
    {
        private readonly IPageRepository _pageRepository;
        private readonly IPageTreeRepository _pageTreeRepository;
        //private readonly ISession _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeGeneratorService"/> class.
        /// </summary>
        /// <param name="pageRepository">The page repository.</param>
        /// <param name="pageTreeRepository">The page tree repository.</param>
        /// <param name="session">The session.</param>
        public TreeGeneratorService(IPageRepository pageRepository, IPageTreeRepository pageTreeRepository/*,
                                    ISession session*/)
        {
            _pageRepository = pageRepository;
            _pageTreeRepository = pageTreeRepository;
            //_session = session;
        }

        #region IAutoSubscriberOf<PageCreated> Members

        /// <summary>
        /// Handle the domain event
        /// </summary>
        /// <param name="e">Domain to process</param>
        public void Handle(PageCreated e)
        {
            _pageTreeRepository.Create(e.Page);
        }

        #endregion

        #region IAutoSubscriberOf<PageDeleted> Members

        /// <summary>
        /// Handle the domain event
        /// </summary>
        /// <param name="e">Domain to process</param>
        public void Handle(PageDeleted e)
        {
            var node = _pageTreeRepository.Get(e.Page.Id);
            if (node != null)
                _pageTreeRepository.Delete(node);
        }

        #endregion

        #region IPageLinkGenerator Members

        public IEnumerable<HtmlLink> CreateLinks(PagePath pagePath, IEnumerable<WikiLink> specifiedLinks)
        {
            var url = VirtualPathUtility.ToAbsolute("~/wiki");
            var found = new List<HtmlLink>();

            var names = specifiedLinks.Select(x => x.PagePath).ToList();
            var foundTreeNodes = _pageTreeRepository.Find(names);
            foreach (var x in foundTreeNodes)
            {
                var title = specifiedLinks.First(y => y.PagePath.Equals(x.Path)).Title;
                if (string.IsNullOrEmpty(title))
                    title = x.Page.Title;

                var link = new HtmlLink(x.Path,
                                        title,
                                        string.Format(@"<a href=""{0}{1}"">{2}</a>", url, x.Path,
                                                      title));
                found.Add(link);
            }

            var missing = (from link in specifiedLinks
                           where
                               !found.Exists(x => x.PagePath.Equals(link.PagePath))
                           select link).ToList();
            foreach (var link in missing)
            {
                var title = !string.IsNullOrEmpty(link.Title) ? link.Title : link.PagePath.Name;
                var ourLink =
                    string.Format(
                        @"<a href=""{0}{1}?title={3}&parentName={4}"" class=""missing"">{2}</a>", url,
                        link.PagePath, title, Uri.EscapeUriString(title),
                        Uri.EscapeUriString(pagePath.ToString()));

                var htmlLink = new HtmlLink(link.PagePath, title, ourLink);
                found.Add(htmlLink);
            }

            return found;
        }

        /// <summary>
        /// Creates a link for the specified page.
        /// </summary>
        /// <param name="page">Page to generate a link for</param>
        /// <returns>Generated link</returns>
        public HtmlLink Create(WikiPage page)
        {
            if (page == null) throw new ArgumentNullException("page");

            var url = VirtualPathUtility.ToAbsolute("~/wiki");
            var treeNode = _pageTreeRepository.Get(page.Id);
            var path = treeNode.Path;
            return new HtmlLink(page.PagePath,
                                        page.Title,
                                        string.Format(@"<a href=""{0}{1}"">{2}</a>", url, path,
                                                      page.Title));
        }
        
        #endregion

        /*
        /// <summary>
        /// Recreate the entire tree.
        /// </summary>
        public void Recreate()
        {
            _pageTreeRepository.DeleteAll();
            var pages = _pageRepository.FindAll();

            // start with all root items.
            foreach (var page in pages.Where(x => x.Parent == null))
            {
                Debug.WriteLine(" => " + page.PageName);
                _pageTreeRepository.Create(page);
                _session.Flush();
                CreateForChildren(page, pages);
            }
        }

        private void CreateForChildren(WikiPage page, IEnumerable<WikiPage> pages)
        {
            foreach (var child in pages.Where(x => x.Parent == page))
            {
                Debug.WriteLine(child.Parent.PageName + " => " + child.PageName);
                _pageTreeRepository.Create(child);
                _session.Flush();
                CreateForChildren(child, pages);
            }
        }
         * */
    }

    public class HtmlLink : IEquatable<HtmlLink>
    {
        public HtmlLink(PagePath pagePath, string title, string link)
        {
            if (pagePath == null) throw new ArgumentNullException("pagePath");
            if (title == null) throw new ArgumentNullException("title");
            if (link == null) throw new ArgumentNullException("link");
            PagePath = pagePath;
            Title = title;
            Link = link;
        }

        public PagePath PagePath { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }

        #region IEquatable<HtmlLink> Members

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(HtmlLink other)
        {
            return PagePath.Equals(other.PagePath);
        }

        #endregion
    }
}