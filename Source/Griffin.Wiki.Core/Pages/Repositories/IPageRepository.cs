using System.Collections.Generic;
using Griffin.Wiki.Core.Pages.DomainModels;
using Griffin.Wiki.Core.Templates.DomainModels;

namespace Griffin.Wiki.Core.Repositories
{
    public interface IPageRepository
    {
        /// <summary>
        /// Create a new page.
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="title"></param>
        /// <param name="pageName"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        WikiPage Create(int parentId, string title, string pageName, PageTemplate template);

        void Delete(string pageName);
        void Delete(WikiPageLink pageName);

        WikiPage Get(string pageName);

        bool Exists(string pageName);

        void Save(WikiPageHistory history);
        void Save(WikiPageLink history);

        void Save(WikiPage page);


        /// <summary>
        ///   Get all pages that links to the specified one.
        /// </summary>
        /// <param name="pageName"> Subject </param>
        /// <returns> A pages that links to the specified one </returns>
        IEnumerable<WikiPage> GetPagesLinkingTo(string pageName);

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

        void AddMissingLinks(WikiPage wikiPage, IEnumerable<string> missingPages);

        /// <summary>
        /// Remove 
        /// </summary>
        /// <param name="pageName"></param>
        void RemoveMissingLinks(string pageName);

        /// <summary>
        /// Find all pages
        /// </summary>
        /// <returns>Pages</returns>
        IEnumerable<WikiPage> FindAll();
    }
}