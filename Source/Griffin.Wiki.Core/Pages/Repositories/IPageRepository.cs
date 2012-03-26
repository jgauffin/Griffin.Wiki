using System.Collections.Generic;
using Griffin.Wiki.Core.Pages.DomainModels;
using Griffin.Wiki.Core.Templates.DomainModels;

namespace Griffin.Wiki.Core.Pages.Repositories
{
    public interface IPageRepository
    {
        /// <summary>
        /// Create a new page.
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="title"></param>
        /// <param name="pagePath"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        WikiPage Create(int parentId, PagePath pagePath, string title, PageTemplate template);

        void Delete(PagePath pagePath);
        void Delete(WikiPageLink link);

        WikiPage Get(PagePath pagePath);

        bool Exists(PagePath pagePath);

        void Save(WikiPageHistory history);
        void Save(WikiPageLink link);

        void Save(WikiPage page);


        /// <summary>
        ///   Get all pages that links to the specified one.
        /// </summary>
        /// <param name="pagePath"> Subject </param>
        /// <returns> A pages that links to the specified one </returns>
        IEnumerable<WikiPage> GetPagesLinkingTo(PagePath pagePath);

        /// <summary>
        /// Fetch a collection of pages
        /// </summary>
        /// <param name="pagePaths">WikiNames for the wanted pages</param>
        /// <returns>A collection of pages.</returns>
        IEnumerable<WikiPage> GetPages(IEnumerable<PagePath> pagePaths);


        /// <summary>
        /// Fetch all pages that links to a missing page
        /// </summary>
        /// <param name="pagePath">WikiName of the missing page</param>
        /// <returns>A collection of referring pages.</returns>
        IEnumerable<MissingPageLink> GetMissingLinks(PagePath pagePath);

        void AddMissingLinks(WikiPage wikiPage, IEnumerable<PagePath> missingPages);

        /// <summary>
        /// Remove 
        /// </summary>
        /// <param name="pagePath"></param>
        void RemoveMissingLinks(PagePath pagePath);

        /// <summary>
        /// Find all pages
        /// </summary>
        /// <returns>Pages</returns>
        IEnumerable<WikiPage> FindAll();

        /// <summary>
        /// Find the 10 most relevant items
        /// </summary>
        /// <param name="term">Partial title or page name</param>
        /// <returns>Matching pages (or an empty collection)</returns>
        IEnumerable<WikiPage> FindTop10(string term);
    }
}