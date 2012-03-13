using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Wiki.Core.DomainModels;
using Griffin.Wiki.Core.Repositories;
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

        public TreeGeneratorService(IPageRepository pageRepository, PageTreeRepository pageTreeRepository)
        {
            _pageRepository = pageRepository;
            _pageTreeRepository = pageTreeRepository;
        }

        /// <summary>
        /// Handle the domain event
        /// </summary>
        /// <param name="e">Domain to process</param>
        public void Handle(PageCreated e)
        {
            Recreate();
            //_pageTreeRepository.Create(e.Page);
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
                _pageTreeRepository.Create(page);
                CreateForChildren(page, pages);
            }

        }

        private void CreateForChildren(WikiPage page, IEnumerable<WikiPage> pages)
        {
            foreach (var child in pages.Where(x=>x.Parent == page))
            {
                _pageTreeRepository.Create(child);
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
