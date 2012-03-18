using System.Collections.Generic;
using System.Linq;
using Griffin.Wiki.Core.DomainModels;
using Griffin.Wiki.Core.Pages.DomainModels;
using Griffin.Wiki.Core.Repositories;
using Griffin.Wiki.Core.SiteMaps.Repositories;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.SiteMaps.Services
{
    /// <summary>
    /// Generates sitemaps for the wiki
    /// </summary>
    [Component]
    public class SiteMapService
    {
        private readonly IPageTreeRepository _pageTreeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteMapService"/> class.
        /// </summary>
        /// <param name="pageTreeRepository">The page tree repository.</param>
        public SiteMapService(IPageTreeRepository pageTreeRepository)
        {
            _pageTreeRepository = pageTreeRepository;
        }

        /// <summary>
        /// Get a complete site map
        /// </summary>
        /// <param name="pageUri">Uri to the action that shows a page</param>
        /// <returns>Hierchical site map</returns>
        /// <example>
        /// var map = siteMapService.Get(Url.Action("Show", "Page"));
        /// </example>
        public IEnumerable<SiteMapNode> Get(string pageUri)
        {
            var completeTree = _pageTreeRepository.FindAll();
            var nodes = new List<SiteMapNode>();
            foreach (var pageTreeNode in completeTree.Where(x => x.Depth == 1))
            {
                var node = new SiteMapNode(pageTreeNode.Page.Title, pageTreeNode.CreateLink(pageUri),
                                           pageTreeNode.CreateLinkPath(pageUri));
                nodes.Add(node);
                AddChildren(pageUri, node, pageTreeNode, completeTree);
            }

            return nodes;
        }

        /// <summary>
        /// Get a site map for three levels only (previous, current, children)
        /// </summary>
        /// <param name="pageName">WikiPageName of the page to generate the partial map for</param>
        /// <param name="pageUri">Uri to the action that shows a page</param>
        /// <returns>Hierchical site map</returns>
        /// <example>
        /// var map = siteMapService.Get("BestPractices", Url.Action("Show", "Page"));
        /// </example>
        public IEnumerable<SiteMapNode> GetPartial(string pageName, string pageUri)
        {
            if (!_pageTreeRepository.Exists(pageName))
                return new LinkedList<SiteMapNode>();

            var completeTree = _pageTreeRepository.GetPartial(pageName);
            var nodes = new List<SiteMapNode>();
            foreach (var pageTreeNode in completeTree.Where(x => x.Depth == 1))
            {
                var node = new SiteMapNode(pageTreeNode.Page.Title, pageTreeNode.CreateLink(pageUri),
                                           pageTreeNode.CreateLinkPath(pageUri));
                nodes.Add(node);
                AddChildren(pageUri, node, pageTreeNode, completeTree);
            }

            return nodes;
        }

        private void AddChildren(string pageUri, SiteMapNode currentMapNode, WikiPageTreeNode currentTreeNode,
                                 IEnumerable<WikiPageTreeNode> completeTree)
        {
            foreach (var childTreeNode in completeTree.Where(x => x.ParentLinage == currentTreeNode.Lineage))
            {
                var child = new SiteMapNode(childTreeNode.Page.Title, childTreeNode.CreateLink(pageUri),
                                            childTreeNode.CreateLinkPath(pageUri));
                currentMapNode.AddChild(child);
                AddChildren(pageUri, child, childTreeNode, completeTree);
            }
        }
    }
}