using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
    public class TreeGeneratorService : IAutoSubscriberOf<PageCreated>, IAutoSubscriberOf<PageDeleted>
    {
        private readonly IPageRepository _pageRepository;
        private readonly PageTreeRepository _pageTreeRepository;
        private readonly ISession _session;

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
            foreach (var page in pages.Where(x=>x.Parent == null))
            {
                Debug.WriteLine(" => " + page.PageName);
                _pageTreeRepository.Create(page);
                _session.Flush();
                CreateForChildren(page, pages);
            }

        }

        private void CreateForChildren(WikiPage page, IEnumerable<WikiPage> pages)
        {
            foreach (var child in pages.Where(x=>x.Parent == page))
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
    }
}
