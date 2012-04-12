using System;
using Griffin.Wiki.Core.Pages.DomainModels;

namespace Griffin.Wiki.Core.Pages.PostLoadProcessors
{
    /// <summary>
    /// Used while processing the page HTML before it's displayed.
    /// </summary>
    public class PostLoadProcessorContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IPostLoadProcessor"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        public PostLoadProcessorContext(WikiPage page, string body)
        {
            if (page == null) throw new ArgumentNullException("page");
            Page = page;
            HtmlBody = body;
        }

        /// <summary>
        /// Gets page being requested
        /// </summary>
        public WikiPage Page { get; private set; }

        /// <summary>
        /// Gets HTML body to return
        /// </summary>
        /// <remarks>This is the property that all processors should process</remarks>
        public string HtmlBody { get; set; }
    }
}