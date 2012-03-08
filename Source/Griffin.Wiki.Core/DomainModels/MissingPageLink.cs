using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Wiki.Core.DomainModels
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
        /// <param name="missingPageName">Name of the missing page.</param>
        public MissingPageLink(WikiPage page, string missingPageName)
        {
            Page = page;
            MissingPageName = missingPageName;
        }

        /// <summary>
        /// Database id
        /// </summary>
        protected int Id { get; set; }

        /// <summary>
        /// Gets or sets page that links to a missing page
        /// </summary>
        public WikiPage Page { get; set; }

        /// <summary>
        /// Gets or sets name of the missing page.
        /// </summary>
        public string MissingPageName { get; set; }
    }
}
