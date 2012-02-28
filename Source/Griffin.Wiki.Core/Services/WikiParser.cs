using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Griffin.Wiki.Core.Repositories;

namespace Griffin.Wiki.Core.Services
{
    /// <summary>
    ///   Parses pure wiki tags
    /// </summary>
    public class WikiParser : IWikiParserResult, IWikiParser
    {
        private readonly IPageRepository _pageRepository;
        private readonly WikiParserConfiguration _configuration;
        private readonly List<string> _references = new List<string>();
        private readonly StringBuilder _sb = new StringBuilder();

        /// <summary>
        ///   Initializes a new instance of the <see cref="WikiParser" /> class.
        /// </summary>
        /// <param name="pageRepository"> The page repository. </param>
        /// <param name="configuration">Configuration used during parsing</param>
        public WikiParser(IPageRepository pageRepository, WikiParserConfiguration configuration)
        {
            if (pageRepository == null) throw new ArgumentNullException("pageRepository");

            _pageRepository = pageRepository;
            _configuration = configuration;
        }

        #region IWikiParserResult Members

        /// <summary>
        ///   Gets all wiki pages that the body links to
        /// </summary>
        public IEnumerable<string> PageLinks
        {
            get { return _references; }
        }

        /// <summary>
        ///   Gets generated content/body.
        /// </summary>
        public string Content
        {
            get { return _sb.ToString(); }
        }

        #endregion

    private string HeadingGenerator(Match match)
    {
        var id = Regex.Replace(match.Groups[2].Value, @"[\W]", "");
        return string.Format(@"<a name=""{1}"" id=""{1}""></a><h{0}>{2}</h{0}>", match.Groups[1].Value, id, match.Groups[2].Value);
    }

        /// <summary>
        ///   Parse the specified html
        /// </summary>
        /// <param name="html"> HTML specified by user (or by a text parser) </param>
        /// <returns>Parsed result</returns>
        public IWikiParserResult Parse(string html)
        {
            if (html == null) throw new ArgumentNullException("html");

            html = Regex.Replace(html, @"<[hH]([1-3])>(.+?)</[hH][1-3]>", HeadingGenerator);


            var lastPos = 0;
            foreach (Match page in Regex.Matches(html, @"\[\[([\w |]+)\]\]"))
            {
                _sb.Append(html.Substring(lastPos, page.Index - lastPos));

                var pair = page.Groups[1].Value.Split('|');
                var name = pair[0];
                var title = pair.Length == 1 ? name : pair[1];

                var link = CreateInternalLink(name, title);
                _sb.Append(link);

                _references.Add(name.ToLower());

                lastPos = page.Index + page.Length;
            }

            _sb.Append(html.Substring(lastPos, html.Length - lastPos));
            return this;
        }

        private string CreateInternalLink(string pageName, string title)
        {
            var pageFormat = _pageRepository.Exists(pageName.ToLower())
                                 ? @"<a href=""{0}page/show/{1}"">{2}</a>"
                                 : @"<a href=""{0}page/create/{1}?title={3}"" class=""missing"">{2}</a>";


            return string.Format(pageFormat, _configuration.RootUri, pageName.ToLower(), title, Uri.EscapeUriString(title));
        }
    }
}