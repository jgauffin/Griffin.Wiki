using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectPortal.Core.Services
{
    /// <summary>
    /// Used to parse the text entered by the user into usable HTML
    /// </summary>
    /// <remarks>Allows user to use their favorite format such as Markdown.</remarks>
    public interface ITextFormatParser
    {
        /// <summary>
        /// Parse the text into HTML
        /// </summary>
        /// <param name="text">Text to parse</param>
        /// <returns>Generated HTML</returns>
        /// <remarks>Should not parse WIKI specific tags such as <c>[[PageName]]</c></remarks>
        string Parse(string text);
    }
}
