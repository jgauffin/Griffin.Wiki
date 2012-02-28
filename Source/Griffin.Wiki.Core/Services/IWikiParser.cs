namespace Griffin.Wiki.Core.Services
{
    /// <summary>
    /// Parses wiki specific tags
    /// </summary>
    public interface IWikiParser
    {
        /// <summary>
        ///   Parse the specified html
        /// </summary>
        /// <param name="html"> HTML specified by user (or by a text parser) </param>
        /// <returns>Parsed result</returns>
        IWikiParserResult Parse(string html);
    }
}