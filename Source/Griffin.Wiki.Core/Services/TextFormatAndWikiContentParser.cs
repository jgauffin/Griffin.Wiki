using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Wiki.Core.Services
{
    /// <summary>
    /// Parses the content (both wiki links and text format)
    /// </summary>
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

        /// <summary>
        /// Converts the used text format to HTML and then parses all wiki specific tags.
        /// </summary>
        /// <param name="content">Content entered by user</param>
        /// <returns>Parsed result</returns>
        public virtual IWikiParserResult Parse(string content)
        {
            if (content == null) throw new ArgumentNullException("content");

            var html = _textFormatParser.Parse(content);
            return _wikiParser.Parse(html);
        }
    }
}
