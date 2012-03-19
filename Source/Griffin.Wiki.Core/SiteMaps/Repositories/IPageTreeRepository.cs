using System.Collections.Generic;
using Griffin.Wiki.Core.Pages.DomainModels;

namespace Griffin.Wiki.Core.SiteMaps.Repositories
{
    public interface IPageTreeRepository
    {
        /// <summary>
        /// Save changes to a tree
        /// </summary>
        /// <param name="node"></param>
        void Save(WikiPageTreeNode node);


        /// <summary>
        /// Get a tree node for the specified page-
        /// </summary>
        /// <param name="pageId">Page db id</param>
        /// <returns>Tree node</returns>
        WikiPageTreeNode Get(int pageId);

        WikiPageTreeNode Create(WikiPage page);
        void Delete(WikiPageTreeNode node);
        void DeleteAll();

        /// <summary>
        /// Find all items
        /// </summary>
        /// <returns>Sorted by depth and titles.</returns>
        IEnumerable<WikiPageTreeNode> FindAll();

        /// <summary>
        /// Find three depths (-1, current, children)
        /// </summary>
        /// <param name="pageName">Page to get map from</param>
        /// <returns>Items sorted by depths and titles</returns>
        IEnumerable<WikiPageTreeNode> GetPartial(string pageName);

        WikiPageTreeNode GetByPath(string relativeUrl);
        WikiPageTreeNode GetByName(string pageName);
        IEnumerable<WikiPageTreeNode> Find(IEnumerable<string> pageNames);
        bool Exists(string pageName);
    }
}