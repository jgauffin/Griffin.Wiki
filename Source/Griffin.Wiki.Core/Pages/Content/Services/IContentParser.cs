namespace Griffin.Wiki.Core.Pages.Content.Services
{
    /// <summary>
    /// Parses the content on a page
    /// </summary>
    public interface IContentParser
    {
        /// <summary>
        /// Converts the used text format to HTML and then parses all wiki specific tags.
        /// </summary>
        /// <param name="pagePath">Path for the page that the content is for</param>
        /// <param name="content">Content entered by user</param>
        /// <returns>Parsed result</returns>
        IWikiParserResult Parse(PagePath pagePath, string content);
    }
}