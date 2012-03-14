using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using Griffin.Wiki.Core.DomainModels;
using Griffin.Wiki.Core.Repositories;
using NHibernate;
using Sogeti.Pattern.DomainEvents;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.Services
{
    /// <summary>
    /// Used to generate and rip down tree.
    /// </summary>
    [Component]
    public class TreeGeneratorService : IAutoSubscriberOf<PageCreated>, IAutoSubscriberOf<PageDeleted>, ILinkGenerator
    {
        private readonly IPageRepository _pageRepository;
        private readonly PageTreeRepository _pageTreeRepository;
        private readonly ISession _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeGeneratorService"/> class.
        /// </summary>
        /// <param name="pageRepository">The page repository.</param>
        /// <param name="pageTreeRepository">The page tree repository.</param>
        /// <param name="session">The session.</param>
        public TreeGeneratorService(IPageRepository pageRepository, PageTreeRepository pageTreeRepository, ISession session)
        {
            _pageRepository = pageRepository;
            _pageTreeRepository = pageTreeRepository;
            _session = session;
        }

        /// <summary>
        /// Handle the domain event
        /// </summary>
        /// <param name="e">Domain to process</param>
        public void Handle(PageCreated e)
        {
            _pageTreeRepository.Create(e.Page);
        }

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

        /// <summary>
        /// Handle the domain event
        /// </summary>
        /// <param name="e">Domain to process</param>
        public void Handle(PageDeleted e)
        {
            var node = _pageRepository.GetTreeNode(e.Page.Id);
            if (node != null)
                _pageTreeRepository.Delete(node);
        }

        public IEnumerable<HtmlLink> CreateLinks(string parentName, IEnumerable<WikiLink> linkNames)
        {
            var root = VirtualPathUtility.ToAppRelative("~/");
            

            var url = VirtualPathUtility.ToAbsolute("~/wiki/");
            var found = (from x in _pageTreeRepository.Find(linkNames.Select(x => x.PageName))
                         select new HtmlLink(x.Page.PageName,
                                             x.Names,
                                             string.Format(@"<a href=""{0}{1}"">{2}</a>", root, x.Names.ToLower(),
                                                           x.Page.Title))
                        ).ToList();


            var missing = (from link in linkNames
                           where !found.Exists(x => x.PageName == link.PageName)
                           let ourLink =
                               string.Format(
                                   @"<a href=""{0}/wiki/{1}?title={3}&parentName={4}"" class=""missing"">{2}</a>", root,
                                   link.PageName.ToLower(), link.Title, Uri.EscapeUriString(link.Title),
                                   Uri.EscapeUriString(parentName))
                           select new HtmlLink(link.PageName, null, ourLink)).ToList();


            found.AddRange(missing);
            return found;
        }
    }

    public class HtmlLink
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
    }

}
