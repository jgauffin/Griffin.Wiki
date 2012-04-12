using System.Collections.Generic;
using Griffin.Wiki.Core.Pages;
using Griffin.Wiki.Core.Pages.Content.Services;
using Griffin.Wiki.Core.Pages.DomainModels;
using Griffin.Wiki.Core.SiteMaps.Services;

namespace Griffin.Wiki.Core
{
    /// <summary>
    /// Used to generate links from page names
    /// </summary>
    public interface IPageLinkGenerator
    {
        /// <summary>
        /// Create links for all child pages of the specified page
        /// </summary>
        /// <param name="page">Page which contains the links</param>
        /// <param name="pagePaths">Collection of paths to pages</param>
        /// <returns>Generated links (for the found pages)</returns>
        IEnumerable<HtmlLink> CreateLinks(PagePath page, IEnumerable<WikiLink> pagePaths);

        /// <summary>
        /// Creates a link with a relative path to the specified page.
        /// </summary>
        /// <param name="sourcePath">Path to the page that the link is generated for</param>
        /// <param name="page">Page which is being linked</param>
        /// <returns>Generated relative link</returns>
        HtmlLink Create(PagePath sourcePath, WikiPage page);

        
    }
}