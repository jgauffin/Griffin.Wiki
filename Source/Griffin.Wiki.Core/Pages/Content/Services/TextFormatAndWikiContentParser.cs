using System;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.Pages.Content.Services
{
    /// <summary>
    /// Parses the content (both wiki links and text format)
    /// </summary>
    [Component]
    public class TextFormatAndWikiContentParser : IContentParser
    {
        private readonly ITextFormatParser _textFormatParser;
        private readonly IWikiParser _wikiParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextFormatAndWikiContentParser"/> class.
        /// </summary>
        /// <param name="textFormatParser">Parsed the used text format.</param>
        /// <param name="wikiParser">Parses WIKI tags.</param>
        public TextFormatAndWikiContentParser(ITextFormatParser textFormatParser, IWikiParser wikiParser)
        {
            _textFormatParser = textFormatParser;
            _wikiParser = wikiParser;
        }

        #region IContentParser Members

        /// <summary>
        /// Converts the used text format to HTML and then parses all wiki specific tags.
        /// </summary>
        /// <param name="pagePath">Page name for the page that the content is for</param>
        /// <param name="content">Content entered by user</param>
        /// <returns>Parsed result</returns>
        public virtual IWikiParserResult Parse(PagePath pagePath, string content)
        {
            if (content == null) throw new ArgumentNullException("content");

            var html = _textFormatParser.Parse(content);
            var result = _wikiParser.Parse(pagePath, html);
            return new WikiParserResult
                       {
                           HtmlBody = result.HtmlBody,
                           OriginalBody = content,
                           PageLinks = result.PageLinks
                       };
        }

        #endregion
    }
}