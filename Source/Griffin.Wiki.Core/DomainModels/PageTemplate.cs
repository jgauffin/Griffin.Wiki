using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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
        public virtual int Id { get; set; }

        /// <summary>
        /// Gets or sets a title describing the template
        /// </summary>
        public virtual string Title { get; protected set; }
        /// <summary>
        /// Gets or sets raw content (Markdown content)
        /// </summary>
        public virtual string Content { get; protected set; }

        /// <summary>
        /// Gets or sets when the template was updated
        /// </summary>
        public virtual DateTime UpdatedAt { get; protected set; }

        /// <summary>
        /// Gets or sets ID of the user that updated the page.
        /// </summary>
        /// <remarks></remarks>
        public virtual string UpdatedBy { get; protected set; }

        /// <summary>
        /// Gets or sets when the item was created
        /// </summary>
        public virtual string CreatedAt { get; protected set; }

        /// <summary>
        /// Gets or sets who created the item
        /// </summary>
        public virtual string CreatedBy { get; protected set; }

        public virtual void Update(string title, string content)
        {
            Title = title;
            Content = content;
            UpdatedAt = DateTime.Now;
            UpdatedBy = Thread.CurrentPrincipal.Identity.Name;
        }
    }
}

