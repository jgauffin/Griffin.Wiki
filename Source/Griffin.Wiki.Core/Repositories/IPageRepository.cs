using System.Collections;
using System.Collections.Generic;
using Griffin.Wiki.Core.DomainModels;

namespace Griffin.Wiki.Core.Repositories
{
    public interface IPageRepository
    {
        bool Exists(string pageName);
        WikiPage Get(string pageName);
        void Save(WikiPage page);
        WikiPage Create(int creator, string title, string pageName);

        /// <summary>
        /// Get all pages that links to the specified one.
        /// </summary>
        /// <param name="pageName">Subject</param>
        /// <returns>A pages that links to the specified one</returns>
        IEnumerable<string> GetLinkingPages(string pageName);

        void Delete(string pageName);
        void Save(WikiPageHistory history);
        void Save(WikiPageLink history);
        void Delete(WikiPageLink pageName);
    }
}