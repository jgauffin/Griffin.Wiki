using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.Pages.PreProcessors
{
    /// <summary>
    /// Adds an ID to each heading
    /// </summary>
    [Component]
    public class HeadingProcessor : IHtmlProcessor
    {
        /// <summary>
        /// Regex used to find allowed 
        /// </summary>
        public const string HeadingRegEx = @"<[hH]([1-3])>(.+?)</[hH][1-3]>";

        /// <summary>
        /// Process body contents
        /// </summary>
        /// <param name="context">Process context</param>
        public void PreProcess(PreProcessorContext context)
        {
            context.Body = Regex.Replace(context.Body, HeadingRegEx, HeadingGenerator);
        }

        private string HeadingGenerator(Match match)
        {
            var id = Regex.Replace(match.Groups[2].Value, @"[\W]", "");
            return string.Format(@"<h{0} id=""{1}"">{2}</h{0}>", match.Groups[1].Value, id, match.Groups[2].Value);
        }
    }
}
