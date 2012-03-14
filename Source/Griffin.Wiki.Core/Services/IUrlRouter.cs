using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Wiki.Core.Services
{
    /// <summary>
    /// Used to generate links from page names
    /// </summary>
    public interface ILinkGenerator
    {
        /// <summary>
        /// Create links for the specified pages
        /// </summary>
        /// <param name="parentName">Parent used for missing pages</param>
        /// <param name="pageNames">Collection of page names</param>
        /// <returns>Generated links (for the found pages)</returns>
        IEnumerable<HtmlLink> CreateLinks(string parentName, IEnumerable<WikiLink> pageNames);
    }
}
