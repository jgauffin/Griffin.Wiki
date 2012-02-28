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
    public class WikiParser : IWikiParserResult
    {
        private readonly IPageRepository _pageRepository;
        private readonly List<string> _references = new List<string>();
        private readonly string _rootUri;
        private readonly StringBuilder _sb = new StringBuilder();

        /// <summary>
        ///   Initializes a new instance of the <see cref="WikiParser" /> class.
        /// </summary>
        /// <param name="pageRepository"> The page repository. </param>
        /// <param name="rootUri"> The root URI for the web site. </param>
        public WikiParser(IPageRepository pageRepository, string rootUri)
        {
            if (pageRepository == null) throw new ArgumentNullException("pageRepository");
            if (rootUri == null) throw new ArgumentNullException("rootUri");

            _pageRepository = pageRepository;
            _rootUri = rootUri;
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

        /// <summary>
        ///   Parse the specified html
        /// </summary>
        /// <param name="html"> HTML specified by user (or by a text parser) </param>
        public void Parse(string html)
        {
            if (html == null) throw new ArgumentNullException("html");

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
        }

        private string CreateInternalLink(string pageName, string title)
        {
            var pageFormat = _pageRepository.Exists(pageName.ToLower())
                                 ? @"<a href=""{0}page/show/{1}"">{2}</a>"
                                 : @"<a href=""{0}page/create/{1}?title={3}"" class=""missing"">{2}</a>";


            return string.Format(pageFormat, _rootUri, pageName.ToLower(), title, Uri.EscapeUriString(title));
        }
    }
}