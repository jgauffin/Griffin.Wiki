using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Wiki.Core.Pages.Repositories;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.Pages.PostProcessors
{
    /// <summary>
    /// Generates a UL list of all immidiate children for a wiki page
    /// </summary>
    [Component]
    public class ChildPageSection : IPostProcessor
    {
        private readonly IPageRepository _repository;
        private readonly ILinkGenerator _linkGenerator;

        public ChildPageSection(IPageRepository repository, ILinkGenerator linkGenerator)
        {
            _repository = repository;
            _linkGenerator = linkGenerator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">Processing context</param>
        /// <returns></returns>
        public void ProcessHtml(PostProcessorContext context)
        {
            int pos = context.HtmlBody.IndexOf("[:child-pages]", System.StringComparison.Ordinal);
            if (pos != -1)
            {
                string html = "<ul>\r\n";
                foreach (var child in context.Page.Children)
                {
                    html += _linkGenerator.Create(child).Link;
                }

                html += "</ul>\r\n";
            }
        }
    }
}
