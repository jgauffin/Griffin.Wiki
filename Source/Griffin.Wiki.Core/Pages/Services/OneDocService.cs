using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Griffin.Wiki.Core.Images.Repositories;
using Griffin.Wiki.Core.Pages.Content.Services;
using Griffin.Wiki.Core.Pages.DomainModels;
using Griffin.Wiki.Core.Pages.Repositories;
using Griffin.Wiki.Core.SiteMaps.DomainModels;
using Griffin.Wiki.Core.SiteMaps.Repositories;
using Griffin.Wiki.Core.SiteMaps.Services;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.Pages.Services
{
    [Component(Lifetime.Transient)]
    public class OneDocService
    {
        private readonly IPageTreeRepository _pageTreeRepository;
        private readonly ITextFormatParser _textParser;
        private readonly IImageRepository _imageRepository;
        private CustomParser _wikiParser;

        public OneDocService(IPageTreeRepository pageTreeRepository, ITextFormatParser textParser, IImageRepository imageRepository)
        {
            _pageTreeRepository = pageTreeRepository;
            _textParser = textParser;
            _imageRepository = imageRepository;
        }

        public string Generate()
        {
            var nodes = _pageTreeRepository.FindAll();
            var home = nodes.First(x => x.Path.ToString() == "/");
            _wikiParser = new CustomParser(new LinkGenerator(nodes));

            StringBuilder sb = new StringBuilder();
            sb.Append(@"<html>
<head>
<title>.NET Developer Guidelines</title>
<style>
.pbr { page-break-after: always; }
code { background: #f0f0f0; }
pre code { display:block; padding: 20px; }
h1,h2,h3,h4,h5,h6 { color: black; }
</style>
</head>
<body>");
            sb.Append(@"<div style=""font-size: 22pt"" class=""bpr"">.NET Developer guidelines</div>");
            GenerateDocument(sb, home.Page, 1);

            sb.Append("</body></html>");
            return sb.ToString();
        }

        private void GenerateDocument(StringBuilder sb, WikiPage page, int heading)
        {

            var html = _textParser.Parse(page.RawBody);
            var result = _wikiParser.Parse(page.PagePath, html);

            var maxHeading = heading;
            var path = page.PagePath.ToString().Replace("/", "_");
            string body;
            if (page.PagePath.ToString() != "/")
            {
                Console.WriteLine(heading + ": " + page.Title);
                sb.AppendFormat(@"<h{0} id=""{2}"">{1}</h{0}>", heading, page.Title, path);
                heading += 1;
                body = Regex.Replace(result.HtmlBody,
                                                    @"<[hH]([1-6]) id=""(.*)"">(.+?)</[hH][1-6]>",
                                                    match =>
                                                    {
                                                        var curHeading = int.Parse(match.Groups[1].Value) + heading;
                                                        if (maxHeading < curHeading)
                                                            maxHeading = curHeading;
                                                        return HeadingGenerator(match, path, heading - 1);
                                                    });

            }
            else
            {
                // root = no title.
                body = result.HtmlBody;
                maxHeading = 1;
            }

            body = FixImages(body);
            sb.Append(body);
            sb.AppendLine(@"<div class=""pbr""></div>");
            sb.AppendLine();

            foreach (var child in page.Children)
            {
                GenerateDocument(sb, child, heading);
            }
        }

        private string FixImages(string body)
        {
            return Regex.Replace(body,
                                               @"<img src=""(/wiki/adm/image/View/(\d+))""",
                                               match =>
                                                   {
                                                       var id = int.Parse(match.Groups[2].Value);
                                                       var img = _imageRepository.Get(id);
                                                       using (var stream = new FileStream("C:\\temp\\html\\img" + id + img.Filename, FileMode.Create))
                                                       {
                                                           img.GetFileStream().CopyTo(stream);
                                                       }

                                                       return string.Format(@"<img src=""{0}""", ("img" + id + img.Filename));
                                                   });
        }

        private string HeadingGenerator(Match match, string docPath, int baseheading)
        {
            var heading = int.Parse(match.Groups[1].Value);
            var result = string.Format(@"<h{0} id={1}>{2}</h{0}>", heading + baseheading, docPath + match.Groups[2].Value, match.Groups[3].Value);
            Console.WriteLine("    " + baseheading + " " + result);
            return result;

        }

        class LinkGenerator : IPageLinkGenerator
        {
            private readonly IEnumerable<WikiPageTreeNode> _allNodes;

            public LinkGenerator(IEnumerable<WikiPageTreeNode> allNodes)
            {
                _allNodes = allNodes;
            }

            /// <summary>
            /// Create links for all child pages of the specified page
            /// </summary>
            /// <param name="page">Page which contains the links</param>
            /// <param name="pagePaths">Collection of paths to pages</param>
            /// <returns>Generated links (for the found pages)</returns>
            public IEnumerable<HtmlLink> CreateLinks(PagePath page, IEnumerable<WikiLink> pagePaths)
            {
                foreach (var wikiLink in pagePaths)
                {
                    var link = _allNodes.FirstOrDefault(x => x.Path.Equals(wikiLink.PagePath));
                    if (link == null)
                    {
                        var title = string.IsNullOrEmpty(wikiLink.Title) ? wikiLink.PagePath.Name : wikiLink.Title;
                        yield return
                            new HtmlLink(wikiLink.PagePath, title,
                                         string.Format(
                                             @"<span style=""color: red; border-bottom: 1px dashed red"">{0}</span>",
                                             title));
                    }
                    else
                    {
                        var title = string.IsNullOrEmpty(wikiLink.Title) ? link.Page.Title : wikiLink.Title;
                        yield return
                            new HtmlLink(wikiLink.PagePath, title,
                                         string.Format(
                                             @"<a href=""#{0}"">{1}</a>",
                                             wikiLink.PagePath.ToString().Replace("/", "_"),
                                             title));

                    }
                }
            }

            /// <summary>
            /// Creates a link for the specified page.
            /// </summary>
            /// <param name="page">Page to generate a link for</param>
            /// <returns>Generated link</returns>
            public HtmlLink Create(WikiPage page)
            {
                return new HtmlLink(page.PagePath, page.Title, '#' + page.PagePath.ToString().Replace("/", "_"));
            }
        }

        class CustomParser : WikiParser
        {
            /// <summary>
            ///   Initializes a new instance of the <see cref="WikiParser" /> class.
            /// </summary>
            /// <param name="pageLinkGenerator">Used to generate links</param>
            public CustomParser(IPageLinkGenerator pageLinkGenerator)
                : base(pageLinkGenerator)
            {
            }
            protected override string CreateLink(PagePath currentPagePath, SiteMaps.Services.HtmlLink link)
            {
                return base.CreateLink(currentPagePath, link);
            }
        }
    }
}
