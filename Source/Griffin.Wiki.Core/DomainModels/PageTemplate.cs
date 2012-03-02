using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Wiki.Core.DomainModels
{

    /// <summary>
    /// Templates are a way to organize content on pages.
    /// </summary>
    /// <returns>
    /// <para>Page templates are used to show the user the kind of information which is expected of them. The structure
    /// is currently not enforced but just added into the editor each time a new page is created.
    /// </para>
    /// <para>
    /// The template is picked from the previous page when created.
    /// </para>
    /// </returns>
    public class PageTemplate
    {
        /// <summary>
        /// Gets or sets database id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a title describing the template
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets raw content (Markdown content)
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets when the template was updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets ID of the user that updated the page.
        /// </summary>
        /// <remarks></remarks>
        public string UpdatedBy { get; set; }
    }
}
