using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Griffin.Wiki.Core.SiteMaps.Services;

namespace Griffin.Wiki.Core.Pages.Content.Services
{
    /// <summary>
    ///   Parses pure wiki tags
    /// </summary>
    /// <remarks>Not thread safe</remarks>
    public class WikiParser : IWikiParser
    {
        private readonly IPageLinkGenerator _pageLinkGenerator;
        private readonly StringBuilder _sb = new StringBuilder();
        private List<PagePath> _references = new List<PagePath>();
        private WikiParserResult _result;

        /// <summary>
        ///   Initializes a new instance of the <see cref="WikiParser" /> class.
        /// </summary>
        /// <param name="pageLinkGenerator">Used to generate links</param>
        public WikiParser(IPageLinkGenerator pageLinkGenerator)
        {
            _pageLinkGenerator = pageLinkGenerator;
        }

        #region IWikiParser Members

        /// <summary>
        ///   Parse the specified content
        /// </summary>
        /// <param name="pagePath">Path to parsed page</param>
        /// <param name="content"> HTML specified by user (or by a text parser) </param>
        /// <returns>Parsed result</returns>
        public IWikiParserResult Parse(PagePath pagePath, string content)
        {
            if (content == null) throw new ArgumentNullException("content");

            if (_result != null)
                throw new InvalidOperationException("Parser may only be used by one thread at a time");

            _result = new WikiParserResult();

            content = Regex.Replace(content, @"<[hH]([1-3])>(.+?)</[hH][1-3]>", HeadingGenerator);

            var pageLinks = Regex.Matches(content, @"\[\[([\w /.!?\*]+)([|]*)([\w ]*)\]\]");

            var linkNames = (from Match match in pageLinks
                             select
                                 new WikiLink
                                     {
                                         PagePath = CreatePath(pagePath, match.Groups[1].Value),
                                         Title = match.Groups[3].Value
                                     }).
                Distinct().ToList();

            var links = _pageLinkGenerator.CreateLinks(pagePath, linkNames);


            var lastPos = 0;
            // three parts: word+whitespace+slash, | (title divider), word+space = title
            foreach (Match match in pageLinks)
            {
                _sb.Append(content.Substring(lastPos, match.Index - lastPos));

                var path = CreatePath(pagePath, match.Groups[1].Value);
                _references.Add(path);

                var link = links.Single(x => x.PagePath.Equals(path));
                var htmlLink = CreateLink(pagePath, link);
                _sb.Append(htmlLink);


                lastPos = match.Index + match.Length;
            }

            _sb.Append(content.Substring(lastPos, content.Length - lastPos));

            _result.PageLinks = _references;
            _result.OriginalBody = content;
            _result.HtmlBody = _sb.ToString().Replace("[\\[", "[[");
            _references = new List<PagePath>();
            var tmp = _result;
            _result = null;
            _sb.Clear();
            return tmp;
        }

        /// <summary>
        /// Generate a link and append it to the string table.
        /// </summary>
        /// <param name="pagePath"></param>
        /// <param name="links"></param>
        /// <param name="match"></param>
        protected virtual string CreateLink(PagePath currentPagePath, HtmlLink link)
        {
            return link.Link;
        }

        private PagePath CreatePath(PagePath parent, string pathOrName)
        {
            if (pathOrName.StartsWith("/"))
                return new PagePath(pathOrName);
            
            return parent.CreateChildPath(pathOrName);
        }


        #endregion

        private string HeadingGenerator(Match match)
        {
            var id = Regex.Replace(match.Groups[2].Value, @"[\W]", "");
            return string.Format(@"<h{0} id=""{1}"">{2}</h{0}>", match.Groups[1].Value, id, match.Groups[2].Value);
        }
    }

    public class WikiParserResult : IWikiParserResult
    {
        #region IWikiParserResult Members

        /// <summary>
        /// Gets all wiki pages that the body links to
        /// </summary>
        public IEnumerable<PagePath> PageLinks { get; set; }

        /// <summary>
        /// Gets generated content/body.
        /// </summary>
        public string HtmlBody { get; set; }

        /// <summary>
        /// Gets content as user typed it
        /// </summary>
        public string OriginalBody { get; set; }

        #endregion
    }

    public class WikiLink : IEquatable<WikiLink>
    {
        public PagePath PagePath { get; set; }
        public string Title { get; set; }

        #region IEquatable<WikiLink> Members

        public bool Equals(WikiLink other)
        {
            if (other == null)
                return false;
            return PagePath.Equals(other.PagePath);
        }

        #endregion
    }
}