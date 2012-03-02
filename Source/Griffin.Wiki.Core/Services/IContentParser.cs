namespace Griffin.Wiki.Core.Services
{
    /// <summary>
    /// Parses the content on a page
    /// </summary>
    public interface IContentParser
    {
        /// <summary>
        /// Converts the used text format to HTML and then parses all wiki specific tags.
        /// </summary>
        /// <param name="pageName">Page name for the page that the content is for</param>
        /// <param name="content">Content entered by user</param>
        /// <returns>Parsed result</returns>
        IWikiParserResult Parse(string pageName, string content);
    }
}