using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Wiki.Core.Pages.DomainModels;

namespace Griffin.Wiki.Core.Pages.PostProcessors
{
    /// <summary>
    /// Use to process a page before it's displayed (i.e. after it has been generated and saved)
    /// </summary>
    public interface IPostProcessor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">Processing context</param>
        /// <returns></returns>
        void ProcessHtml(PostProcessorContext context);
    }

    /// <summary>
    /// Used while processing the page HTML before it's displayed.
    /// </summary>
    public class PostProcessorContext
    {
        /// <summary>
        /// Gets page being requested
        /// </summary>
        public WikiPage Page { get; private set; }

        /// <summary>
        /// Gets HTML body to return
        /// </summary>
        public string HtmlBody { get; set; }
    }
}
