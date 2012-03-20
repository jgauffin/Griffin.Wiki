using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Griffin.Wiki.Core.Pages.Content.Services;
using Griffin.Wiki.Core.Pages.DomainModels;
using Griffin.Wiki.Core.Pages.DomainModels.Events;
using Griffin.Wiki.Core.Pages.Repositories;
using Griffin.Wiki.Core.Services;
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
    public class TreeGeneratorService : IAutoSubscriberOf<PageCreated>, IAutoSubscriberOf<PageDeleted>, ILinkGenerator
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

        #region ILinkGenerator Members

        public IEnumerable<HtmlLink> CreateLinks(string parentName, IEnumerable<WikiLink> specifiedLinks)
        {
            var url = VirtualPathUtility.ToAbsolute("~/wiki");
            var found = new List<HtmlLink>();

            var names = specifiedLinks.Select(x => x.PageName).ToList();
            var foundTreeNodes = _pageTreeRepository.Find(names);
            foreach (var x in foundTreeNodes)
            {
                var path = x.Names.ToLower().Replace("Home/", "");
                var link = new HtmlLink(x.Page.PageName,
                                        x.Names,
                                        string.Format(@"<a href=""{0}{1}"">{2}</a>", url, path,
                                                      x.Page.Title));
                found.Add(link);
            }

            var missing = (from link in specifiedLinks
                           where
                               !found.Exists(x => x.PageName.Equals(link.PageName, StringComparison.OrdinalIgnoreCase))
                           select link).ToList();
            foreach (var link in missing)
            {
                var ourLink =
                    string.Format(
                        @"<a href=""{0}/{1}?title={3}&parentName={4}"" class=""missing"">{2}</a>", url,
                        link.PageName.ToLower(), link.Title, Uri.EscapeUriString(link.Title),
                        Uri.EscapeUriString(parentName));

                var htmlLink = new HtmlLink(link.PageName, null, ourLink);
                found.Add(htmlLink);
            }

            return found;
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
        public HtmlLink(string name, string path, string link)
        {
            PageName = name;
            Path = path;
            Link = link;
        }

        public string PageName { get; set; }
        public string Path { get; set; }
        public string Link { get; set; }

        #region IEquatable<HtmlLink> Members

        public bool Equals(HtmlLink other)
        {
            return PageName.Equals(other.PageName, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}