namespace Griffin.Wiki.Core.Pages.Content.Services
{
    /// <summary>
    /// Parses wiki specific tags
    /// </summary>
    public interface IWikiParser
    {
        /// <summary>
        ///   Parse the specified content
        /// </summary>
        /// <param name="pagePath">Path to the page that the content is for</param>
        /// <param name="html"> HTML specified by user (or by a text parser) </param>
        /// <returns>Parsed result</returns>
        IWikiParserResult Parse(PagePath pagePath, string html);
    }
}