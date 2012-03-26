namespace Griffin.Wiki.Core.Pages.DomainModels
{
    /// <summary>
    /// Keeps track of pages that links to missing pages
    /// </summary>
    public class MissingPageLink
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingPageLink"/> class.
        /// </summary>
        /// <param name="page">Page that links to a missing page.</param>
        /// <param name="missingPagePath">Name of the missing page.</param>
        public MissingPageLink(WikiPage page, PagePath missingPagePath)
        {
            Page = page;
            MissingPagePath = missingPagePath.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingPageLink"/> class.
        /// </summary>
        protected MissingPageLink()
        {
        }

        /// <summary>
        /// Database id
        /// </summary>
        protected virtual int Id { get; set; }

        /// <summary>
        /// Gets or sets page that links to a missing page
        /// </summary>
        public virtual WikiPage Page { get; set; }

        /// <summary>
        /// Gets or sets name of the missing page.
        /// </summary>
        public virtual string MissingPagePath { get; protected set; }
    }
}