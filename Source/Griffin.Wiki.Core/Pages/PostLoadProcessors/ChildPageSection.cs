using Griffin.Wiki.Core.Pages.Repositories;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.Pages.PostLoadProcessors
{
    /// <summary>
    /// Generates a UL list of all immidiate children for a wiki page
    /// </summary>
    [Component]
    public class ChildPageSection : IPostLoadProcessor
    {
        private readonly IPageRepository _repository;

        public ChildPageSection(IPageRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">Processing context</param>
        /// <returns></returns>
        public void ProcessHtml(PostLoadProcessorContext context)
        {
            int pos = context.HtmlBody.IndexOf("[:child-pages]", System.StringComparison.Ordinal);
            if (pos != -1)
            {
                string html = @"<ul class=""child-pages"">";
                foreach (var child in context.Page.Children)
                {
                    var relative = context.Page.PagePath.GetPathRelativeTo(child.PagePath);
                    html += string.Format(@"<li><a href=""{0}/"" class=""wiki-link"">{1}</a></li>", relative, child.Title);
                }

                html += "</ul>\r\n";
                context.HtmlBody = context.HtmlBody.Substring(0, pos) + html + context.HtmlBody.Substring(pos + 14);
            }
        }
    }
}
