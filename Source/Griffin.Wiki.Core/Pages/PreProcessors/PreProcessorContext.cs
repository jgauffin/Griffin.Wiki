using System;
using System.Collections.Generic;
using Griffin.Wiki.Core.Pages.DomainModels;

namespace Griffin.Wiki.Core.Pages.PreProcessors
{
    /// <summary>
    /// 
    /// </summary>
    public class PreProcessorContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreProcessorContext"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        public PreProcessorContext(WikiPage page, string content)
        {
            if (page == null) throw new ArgumentNullException("page");

            Page = page;
            OriginalBody = content;
            Body = content;
            LinkedPages = new List<PagePath>();
        }

        public string OriginalBody { get; private set; }

        /// <summary>
        /// Gets page that the body belongs to.
        /// </summary>
        /// <remarks>Should only be used as an information source and should not be modified by the processors</remarks>
        public WikiPage Page { get; private set; }

        /// <summary>
        /// Gets generated HTML body
        /// </summary>
        /// <remarks>Body which will be </remarks>
        public string Body { get; set; }

        /// <summary>
        /// Gets pages that are linked in the body.
        /// </summary>
        public List<PagePath> LinkedPages { get; private set; }
    }
}