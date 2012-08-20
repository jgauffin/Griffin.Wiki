using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Griffin.Logging;
using Griffin.Wiki.Core.Images.Repositories;
using Griffin.Wiki.Core.Pages.Content.Services;
using Griffin.Wiki.Core.Pages.DomainModels;
using Griffin.Wiki.Core.Pages.PostLoadProcessors;
using Griffin.Wiki.Core.Pages.PreProcessors;
using Griffin.Wiki.Core.Pages.Repositories;
using Griffin.Wiki.Core.SiteMaps.DomainModels;
using Griffin.Wiki.Core.SiteMaps.Repositories;
using Griffin.Wiki.Core.SiteMaps.Services;
using Griffin.Container;

namespace Griffin.Wiki.Core.Pages.Services
{
    [Component(Lifetime = Lifetime.Transient)]
    public class OneDocService
    {
        private readonly IPageTreeRepository _pageTreeRepository;
        private readonly IPreProcessorService _preProcesor;
        private readonly IPostLoadProcessService _postProcessor;
        private readonly IImageRepository _imageRepository;
        private ILogger _logger = LogManager.GetLogger<OneDocService>();

        public OneDocService(IPageTreeRepository pageTreeRepository, IPreProcessorService preProcesor,
                             IImageRepository imageRepository, IPostLoadProcessService postProcessor)
        {
            _pageTreeRepository = pageTreeRepository;
            _preProcesor = preProcesor;
            _imageRepository = imageRepository;
            _postProcessor = postProcessor;
        }

        public void GeneratePDF(string appDataDirectory, Stream outputStream)
        {
            if (!appDataDirectory.EndsWith("\\"))
                appDataDirectory += "\\";

            var workingDirectory = Path.Combine(appDataDirectory, Path.GetFileNameWithoutExtension(Path.GetTempFileName()));
            Directory.CreateDirectory(workingDirectory);

            CopyTool(appDataDirectory, workingDirectory);


            using (var stream = new FileStream(Path.Combine(workingDirectory, "index.html"), FileMode.CreateNew))
            {
                GenerateHTML(workingDirectory, new StreamWriter(stream));
            }


            RunTool(workingDirectory);

            using (var pdfStream = new FileStream(Path.Combine(workingDirectory, "wiki.pdf"), FileMode.Open))
            {
                pdfStream.CopyTo(outputStream);
            }

            new Timer(DeleteWorkingDir, workingDirectory, TimeSpan.FromSeconds(5), new TimeSpan(-1));
        }

        private void DeleteWorkingDir(object state)
        {
            try
            {
                Directory.Delete((string)state, true);
            }
            catch (Exception err)
            {
                _logger.Error("Failed to delete working dir " + state, err);
            }
        }

        private static void RunTool(string workingDirectory)
        {
            var psi = new ProcessStartInfo(Path.Combine(workingDirectory, "wkhtmltopdf.exe"), "index.html wiki.pdf") { WorkingDirectory = workingDirectory, UseShellExecute = true,CreateNoWindow = true};
            using (var process = Process.Start(psi))
            {
                process.WaitForExit();
            }
        }

        private static void CopyTool(string appDataDirectory, string workingDirectory)
        {
            var toolDir = Path.Combine(appDataDirectory, "wkhtmltopdf\\");
            foreach (var file in Directory.GetFiles(toolDir, "*.*"))
            {
                File.Copy(file, Path.Combine(workingDirectory, Path.GetFileName(file)));
            }
        }

        public void GenerateHTML(string workingDirectory, TextWriter writer)
        {
            if (!workingDirectory.EndsWith("\\"))
                workingDirectory += "\\";

            var nodes = _pageTreeRepository.FindAll();
            var home = nodes.First(x => x.Path.ToString() == "/");

            writer.WriteLine(
                @"<html>
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
            writer.WriteLine(@"<h1>.NET Developer guidelines</h1>");
            GenerateDocument(workingDirectory, writer, home.Page, 1);

            writer.WriteLine("</body></html>");
        }

        private int _page = 1;
        private void GenerateDocument(string workingDirectory, TextWriter writer, WikiPage page, int heading)
        {
            var preContext = new PreProcessorContext(page, page.RawBody);
            _preProcesor.Invoke(preContext);

            var postContext = new PostLoadProcessorContext(page, preContext.Body);
            _postProcessor.Process(postContext);

            var maxHeading = heading;
            var path = page.PagePath.ToString().Replace("/", "_");
            string body;
            if (page.PagePath.ToString() != "/")
            {
                Console.WriteLine(heading + ": " + page.Title);
                writer.Write(@"<h{0} id=""{2}"">{1}</h{0}>", heading, page.Title, path);
                heading += 1;
                body = Regex.Replace(postContext.HtmlBody,
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
                body = Regex.Replace(postContext.HtmlBody,
                                     @"<[hH]([1-6]) id=""(.*)"">(.+?)</[hH][1-6]>",
                                     match => HeadingGenerator(match, path, 1));
                // root = no title.
                maxHeading = 1;
            }

            // Convert links
            body = Regex.Replace(body, @"<a href=""(.*?)"" class=""(.*?)""", m => ReplaceLinks(page, m));

            body = FixImages(workingDirectory, body);
            writer.Write(body);
            writer.WriteLine(@"<div class=""pbr""></div>");
            writer.WriteLine("<!-------------------------------- Page " + (_page++) + "--------------------------->");
            writer.WriteLine();

            foreach (var child in page.Children)
            {
                GenerateDocument(workingDirectory, writer, child, heading);
            }
        }

        private string ReplaceLinks(WikiPage page, Match match)
        {
            if (!match.Groups[2].Value.Contains("wiki-link"))
                return match.Groups[0].Value;

            var uri = match.Groups[1].Value;
            var pos = uri.IndexOf('?');
            var query = "";
            if (pos != -1)
            {
                query = uri.Substring(pos);
                uri = uri.Remove(pos);
            }

            var path = new RelativePagePath(page.PagePath, uri);
            return string.Format(@"<a href=""#{0}{2}"" class=""{1}""", path.ToAbsolute().ToString().Replace("/", "_"), match.Groups[2].Value, query);
        }

        private string FixImages(string workingDirectory, string body)
        {
            return Regex.Replace(body,
                                 @"<img src=""(/wiki/adm/image/View/(\d+))""",
                                 match =>
                                 {
                                     var id = int.Parse(match.Groups[2].Value);
                                     var img = _imageRepository.Get(id);

                                     var filename = string.Format("{0}img{1}{2}", workingDirectory, id, img.Filename);

                                     if (img.ContentType.Contains("jpeg"))
                                     {
                                         filename = filename.Replace(".jpg", ".png").Replace(".jpg", ".png");
                                         var png = Image.FromStream(img.GetFileStream());
                                         using (var stream = new FileStream(filename, FileMode.Create))
                                         {
                                             png.Save(stream, ImageFormat.Png);
                                         }
                                     }
                                     else
                                     {
                                         using (var stream = new FileStream(filename, FileMode.Create))
                                         {
                                             img.GetFileStream().CopyTo(stream);
                                         }

                                     }

                                     return string.Format(@"<img src=""{0}""", Path.GetFileName(filename));
                                 });
        }

        private string HeadingGenerator(Match match, string docPath, int baseheading)
        {
            var heading = int.Parse(match.Groups[1].Value);
            var result = string.Format(@"<h{0} id={1}>{2}</h{0}>", heading + baseheading,
                                       docPath + match.Groups[2].Value, match.Groups[3].Value);
            Console.WriteLine("    " + baseheading + " " + result);
            return result;

        }

        private class LinkGenerator : IPageLinkGenerator
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
            /// <param name="sourcePath">Path for the page that links to the specified page</param>
            /// <param name="page">Page to generate a link for</param>
            /// <returns>Generated link</returns>
            public HtmlLink Create(PagePath sourcePath, WikiPage page)
            {
                return new HtmlLink(page.PagePath, page.Title, '#' + page.PagePath.ToString().Replace("/", "_"));
            }
        }
    }
}
