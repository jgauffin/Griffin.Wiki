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
        IEnumerable<string> GetLinkingPages(string pageName);

        WikiPageTreeNode GetTreeNode(int pageId);
    }
}