namespace Griffin.Wiki.Core.Services
{
    /// <summary>
    /// Parses wiki specific tags
    /// </summary>
    public interface IWikiParser
    {
        /// <summary>
        ///   Parse the specified content
        /// </summary>
        /// <param name="currentPageName">Page that the content is for</param>
        /// <param name="html"> HTML specified by user (or by a text parser) </param>
        /// <returns>Parsed result</returns>
        IWikiParserResult Parse(string currentPageName, string html);
    }
}