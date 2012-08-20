using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Griffin.Wiki.Core.Pages.Content.Services;
using Griffin.Wiki.Core.SiteMaps.Repositories;
using Griffin.Container;

namespace Griffin.Wiki.Core.Pages.PreProcessors
{
    /// <summary>
    ///   Converts wiki links into relative HTML links
    /// </summary>
    [Component]
    public class WikiLinkProcessor : ITextProcessor
    {
        /// <summary>
        ///   Regex used to identity wiki page links
        /// </summary>
        public const string PageLinkRegEx = @"\[\[([^|\]]+)([|]*)([^]]*)\]\]";

        private readonly IPageTreeRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="WikiLinkProcessor"/> class.
        /// </summary>
        /// <param name="repository">Nodes are used to lookup the linked pages (to create relative links).</param>
        public WikiLinkProcessor(IPageTreeRepository repository)
        {
            _repository = repository;
        }

        #region ITextProcessor Members

        public void PreProcess(PreProcessorContext context)
        {
            var regexLinks = Regex.Matches(context.Body, PageLinkRegEx);

            // Prescan to generate links
            var links = BuildLinkList(context, regexLinks);


            var lastPos = 0;
            var sb = new StringBuilder(context.Body.Length + 1000);
            var index = 0;
            foreach (Match match in regexLinks)
            {
                sb.Append(context.Body.Substring(lastPos, match.Index - lastPos));

                var link = links[index];
                if (!context.LinkedPages.Contains(link.PagePath))
                    context.LinkedPages.Add(link.PagePath);

                var htmlLink = CreateHtmlLink(context.Page.PagePath, link);
                sb.Append(htmlLink);


                lastPos = match.Index + match.Length;
                ++index;
            }

            sb.Append(context.Body.Substring(lastPos, context.Body.Length - lastPos));


            context.Body = sb.ToString().Replace(@"[\[", "[[");
        }

        #endregion

        private string CreateHtmlLink(PagePath parsedPage, WikiLink link)
        {
            if (link.Exists)
            {
                return string.Format(@"<a href=""{0}"" class=""wiki-link"">{1}</a>", parsedPage.GetPathRelativeTo(link.PagePath), link.Title);
            }

            return string.Format(
                @"<a href=""{0}?title={1}"" class=""wiki-link missing"">{2}</a>", parsedPage.GetPathRelativeTo(link.PagePath),
                link.Title, link.Title);
        }

        private List<WikiLink> BuildLinkList(PreProcessorContext context, MatchCollection regExLinks)
        {
            var wikiLinks = (from Match match in regExLinks
                             select new WikiLink
                                        {
                                            PagePath = CreatePath(context.Page.PagePath, match.Groups[1].Value),
                                            Title = match.Groups[3].Value
                                        }).ToList();

            var treeNodes = _repository.Find(wikiLinks.Select(x => x.PagePath).ToList());
            foreach (var link in wikiLinks)
            {
                var node = treeNodes.FirstOrDefault(x => x.Path.Equals(link.PagePath));
                if (node != null)
                {
                    if (string.IsNullOrEmpty(link.Title))
                        link.Title = node.Page.Title;
                }

                link.Exists = node != null;
            }

            foreach (var link in wikiLinks)
            {
                if (string.IsNullOrEmpty(link.Title))
                    link.Title = link.PagePath.Name;
            }

            return wikiLinks;
        }

        private PagePath CreatePath(PagePath pagePath, string linkPath)
        {
            // child page
            if (!linkPath.Contains("..") && !linkPath.Contains("/"))
                return new PagePath(pagePath + linkPath + "/");

            if (linkPath.Contains(".."))
                return new RelativePagePath(pagePath, linkPath).ToAbsolute();

            return linkPath.StartsWith("/")
                       ? new PagePath(linkPath)
                       : pagePath.CreateChildPath(linkPath);
        }
    }
}