using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Wiki.Core.Pages.DomainModels;

namespace Griffin.Wiki.Core.Pages.PreProcessors
{
    /// <summary>
    /// Used to parse the body as the user entered it
    /// </summary>
    public interface ITextProcessor
    {
        /// <summary>
        /// Processes the body contents
        /// </summary>
        /// <param name="context">Processing context</param>
        void PreProcess(PreProcessorContext context);
    }

    /// <summary>
    /// Processes the body after any <see cref="ITextProcessor"/>. 
    /// </summary>
    /// <remarks>The body is HTML content as this stage</remarks>
    public interface IHtmlProcessor
    {
        /// <summary>
        /// Process body contents
        /// </summary>
        /// <param name="context">Process context</param>
        void PreProcess(PreProcessorContext context);
    }

    /// <summary>
    /// 
    /// </summary>
    public class PreProcessorContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreProcessorContext"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        public PreProcessorContext(WikiPage page)
        {
            if (page == null) throw new ArgumentNullException("page");

            Page = page;
            LinkedPages = new List<PagePath>();
        }

        /// <summary>
        /// Gets page that the body belongs to.
        /// </summary>
        /// <remarks>Should only be used as an information source and should not be modified by the processors</remarks>
        public WikiPage Page { get; private set; }

        /// <summary>
        /// Gets processed body
        /// </summary>
        /// <remarks>Body which will be </remarks>
        public string Body { get; set; }

        /// <summary>
        /// Gets pages that are linked in the body.
        /// </summary>
        public List<PagePath> LinkedPages { get; private set; }
    }
}
