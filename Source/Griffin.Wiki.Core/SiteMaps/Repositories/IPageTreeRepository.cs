using System.Collections.Generic;
using Griffin.Wiki.Core.Pages;
using Griffin.Wiki.Core.Pages.DomainModels;
using Griffin.Wiki.Core.SiteMaps.DomainModels;

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
        IEnumerable<WikiPageTreeNode> FindAll(FindOption option = FindOption.Default);

        /// <summary>
        /// Find three depths (-1, current, children)
        /// </summary>
        /// <param name="pagePath">Page to get map from</param>
        /// <returns>Items sorted by depths and titles</returns>
        IEnumerable<WikiPageTreeNode> GetPartial(PagePath pagePath);

        WikiPageTreeNode GetByPath(string relativeUrl);
        WikiPageTreeNode GetByPath(PagePath pagePath);
        IEnumerable<WikiPageTreeNode> Find(IEnumerable<PagePath> pages);
        bool Exists(PagePath pagePath);
    }

    public enum FindOption
    {
        Default,
        LoadPages
    }
}