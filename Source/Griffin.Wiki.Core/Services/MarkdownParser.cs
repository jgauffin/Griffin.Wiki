using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarkdownDeep;

namespace ProjectPortal.Core.Services
{
    /// <summary>
    /// Parses markdown
    /// </summary>
    public class MarkdownParser : ITextFormatParser
    {
        /// <summary>
        /// Parse the text into HTML
        /// </summary>
        /// <param name="text">Text to parse</param>
        /// <returns>Generated HTML</returns>
        /// <remarks>Should not parse WIKI specific tags such as <c>[[PageName]]</c></remarks>
        public string Parse(string text)
        {
            var md = new Markdown { ExtraMode = true };
            return md.Transform(text);
        }
    }
}
