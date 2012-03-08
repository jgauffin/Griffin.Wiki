using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Griffin.Wiki.Core.DomainModels;
using Griffin.Wiki.Core.Repositories;

namespace Griffin.Wiki.Core.Services
{
    /// <summary>
    ///   Parses pure wiki tags
    /// </summary>
    /// <remarks>Not thread safe</remarks>
    public class WikiParser : IWikiParser
    {
        private readonly IPageRepository _pageRepository;
        private readonly WikiParserConfiguration _configuration;
        private List<string> _references = new List<string>();
        private readonly StringBuilder _sb = new StringBuilder();
        private WikiParserResult _result;

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

        private string HeadingGenerator(Match match)
        {
            var id = Regex.Replace(match.Groups[2].Value, @"[\W]", "");
            return string.Format(@"<a name=""{1}"" id=""{1}""></a><h{0}>{2}</h{0}>", match.Groups[1].Value, id, match.Groups[2].Value);
        }

        /// <summary>
        ///   Parse the specified content
        /// </summary>
        /// <param name="currentPageName">Page that the body belongs to</param>
        /// <param name="content"> HTML specified by user (or by a text parser) </param>
        /// <returns>Parsed result</returns>
        public IWikiParserResult Parse(string currentPageName, string content)
        {
            if (content == null) throw new ArgumentNullException("content");

            if (_result != null)
                throw new InvalidOperationException("Parser may only be used by one thread at a time");

            _result = new WikiParserResult();

            content = Regex.Replace(content, @"<[hH]([1-3])>(.+?)</[hH][1-3]>", HeadingGenerator);


            var lastPos = 0;
            foreach (Match page in Regex.Matches(content, @"\[\[([\w |]+)\]\]"))
            {
                _sb.Append(content.Substring(lastPos, page.Index - lastPos));

                var pair = page.Groups[1].Value.Split('|');
                var name = pair[0];
                var title = pair.Length == 1 ? name : pair[1];

                var link = CreateInternalLink(name, title, currentPageName);
                _sb.Append(link);

                _references.Add(name.ToLower());

                lastPos = page.Index + page.Length;
            }

            _sb.Append(content.Substring(lastPos, content.Length - lastPos));

            _result.PageLinks = _references;
            _result.OriginalBody = content;
            _result.HtmlBody = _sb.ToString();
            _references = new List<string>();
            var tmp = _result;
            _result = null;
            return tmp;
        }

        private string CreateInternalLink(string pageName, string title, string parentName)
        {
            var pageFormat = _pageRepository.Exists(pageName.ToLower())
                                 ? @"<a href=""{0}page/show/{1}"">{2}</a>"
                                 : @"<a href=""{0}page/create/{1}?title={3}&parentName={4}"" class=""missing"">{2}</a>";


            return string.Format(pageFormat, _configuration.RootUri, pageName.ToLower(), title, Uri.EscapeUriString(title), Uri.EscapeUriString(parentName));
        }
    }

    public class WikiParserResult : IWikiParserResult
    {
        /// <summary>
        /// Gets all wiki pages that the body links to
        /// </summary>
        public IEnumerable<string> PageLinks { get; set; }

        /// <summary>
        /// Gets generated content/body.
        /// </summary>
        public string HtmlBody { get; set; }

        /// <summary>
        /// Gets content as user typed it
        /// </summary>
        public string OriginalBody { get; set; }
    }
}