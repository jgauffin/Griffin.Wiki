using System;

namespace ProjectPortal.Core.DomainModels
{
    /// <summary>
    /// Link from one page to another.
    /// </summary>
    public class WikiPageLink
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WikiPageLink"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="linkedPage">The linked page.</param>
        public WikiPageLink(WikiPage page, WikiPage linkedPage)
        {
            if (page == null) throw new ArgumentNullException("page");
            if (linkedPage == null) throw new ArgumentNullException("linkedPage");
            Page = page;
            LinkedPage = linkedPage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WikiPageLink"/> class.
        /// </summary>
        protected WikiPageLink()
        {
            
        }

        /// <summary>
        /// Gets page that links to another page
        /// </summary>
        public virtual WikiPage Page { get; protected set; }

        /// <summary>
        /// Gets page that the current page links to
        /// </summary>
        public virtual WikiPage LinkedPage { get; protected set; }
    }
}