using System.Collections.Generic;
using Griffin.Wiki.Core.DomainModels;

namespace Griffin.Wiki.Core.Repositories
{
    public interface IPageRepository
    {
        WikiPage Create(int creator, string title, string pageName);

        void Delete(string pageName);
        void Delete(WikiPageLink pageName);
        void Delete(WikiPageTreeNode node);

        WikiPage Get(string pageName);

        bool Exists(string pageName);

        void Save(WikiPageHistory history);
        void Save(WikiPageLink history);
        void Save(WikiPageTreeNode node);
        
        void Save(WikiPage page);
        

        /// <summary>
        ///   Get all pages that links to the specified one.
        /// </summary>
        /// <param name="pageName"> Subject </param>
        /// <returns> A pages that links to the specified one </returns>
        IEnumerable<WikiPage> GetPagesLinkingTo(string pageName);

        /// <summary>
        /// Get a tree node for the specified page-
        /// </summary>
        /// <param name="pageId">Page db id</param>
        /// <returns>Tree node</returns>
        WikiPageTreeNode GetTreeNode(int pageId);


        /// <summary>
        /// Fetch a collection of pages
        /// </summary>
        /// <param name="pageNames">WikiNames for the wanted pages</param>
        /// <returns>A collection of pages.</returns>
        IEnumerable<WikiPage> GetPages(IEnumerable<string> pageNames);


        /// <summary>
        /// Fetch all pages that links to a missing page
        /// </summary>
        /// <param name="pageName">WikiName of the missing page</param>
        /// <returns>A collection of referring pages.</returns>
        IEnumerable<MissingPageLink> GetMissingLinks(string pageName);
    }
}