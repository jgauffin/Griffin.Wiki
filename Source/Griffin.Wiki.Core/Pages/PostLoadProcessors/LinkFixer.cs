using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Griffin.Container;

namespace Griffin.Wiki.Core.Pages.PostLoadProcessors
{
    /// <summary>
    /// Fixes links with double slashes.
    /// </summary>
    [Component]
    public class LinkFixer : IPostLoadProcessor
    {
        public void ProcessHtml(PostLoadProcessorContext context)
        {
            context.HtmlBody = Regex.Replace(context.HtmlBody, @"<a href=""(.*)""", Evaluator);
        }

        private string Evaluator(Match match)
        {
            if (!match.Groups[0].Value.Contains("wiki-link")
                && !match.Groups[1].Value.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return "http://" + match.Groups[0].Value.Replace("//", "/");
            }

            // replace links with double slashes, but not in "http://"
            int pos = match.Groups[0].Value.IndexOf("//");
            if (pos != -1 && match.Groups[0].Value[pos-1] != ':')
                return match.Groups[0].Value.Replace("//", "/");

            return match.Groups[0].Value;
        }
    }
}
