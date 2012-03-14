﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Griffin.Wiki.Core.DomainModels;
using Griffin.Wiki.Core.Repositories;
using Griffin.Wiki.Core.Repositories.Documents;

namespace Griffin.Wiki.Core.Services
{
    /// <summary>
    ///   Parses pure wiki tags
    /// </summary>
    /// <remarks>Not thread safe</remarks>
    public class WikiParser : IWikiParser
    {
        private readonly ILinkGenerator _linkGenerator;
        private List<string> _references = new List<string>();
        private readonly StringBuilder _sb = new StringBuilder();
        private WikiParserResult _result;

        /// <summary>
        ///   Initializes a new instance of the <see cref="WikiParser" /> class.
        /// </summary>
        /// <param name="linkGenerator">Used to generate links</param>
        public WikiParser(ILinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        private string HeadingGenerator(Match match)
        {
            var id = Regex.Replace(match.Groups[2].Value, @"[\W]", "");
            return string.Format(@"<h{0} id=""{1}"">{2}</h{0}>", match.Groups[1].Value, id, match.Groups[2].Value);
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

            var linkNames = (from Match page in Regex.Matches(content, @"\[\[([\w |]+)\]\]")
                             select page.Groups[1].Value.Split('|')
                             into pair
                             select new WikiLink {PageName = pair[0], Title = pair.Length == 1 ? pair[0] : pair[1]}).ToList();

            var links = _linkGenerator.CreateLinks(currentPageName, linkNames);


            var lastPos = 0;
            foreach (Match page in Regex.Matches(content, @"\[\[([\w |]+)\]\]"))
            {
                _sb.Append(content.Substring(lastPos, page.Index - lastPos));

                var pair = page.Groups[1].Value.Split('|');
                var name = pair[0];
                var link = links.Single(x => x.PageName == name);
                _sb.Append(link.Link);

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

    public class WikiLink
    {
        public string PageName { get; set; }
        public string Title { get; set; }
    }
}