using System.Collections.Generic;

namespace Griffin.Wiki.Core.Pages.Content.Services
{
    /// <summary>
    /// Result generated by the Wiki parser.
    /// </summary>
    public interface IWikiParserResult
    {
        /// <summary>
        /// Gets name of all wiki pages that the body links to.
        /// </summary>
        IEnumerable<string> PageLinks { get; }

        /// <summary>
        /// Gets generated content/body.
        /// </summary>
        string HtmlBody { get; }

        /// <summary>
        /// Gets content as user typed it
        /// </summary>
        string OriginalBody { get; }
    }
}