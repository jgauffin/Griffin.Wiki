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

        public TreeGeneratorService(IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }

        /// <summary>
        /// Handle the domain event
        /// </summary>
        /// <param name="e">Domain to process</param>
        public void Handle(PageCreated e)
        {
            var parentNode = _pageRepository.GetTreeNode(e.Page.Parent.Id);
            var ourNode = new WikiPageTreeNode(e.Page, parentNode);
            _pageRepository.Save(ourNode);
        }

        /// <summary>
        /// Handle the domain event
        /// </summary>
        /// <param name="e">Domain to process</param>
        public void Handle(PageDeleted e)
        {
            var node = _pageRepository.GetTreeNode(e.Page.Id);
            if (node != null)
                _pageRepository.Delete(node);
        }
    }
}
