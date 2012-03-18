using System;
using Griffin.Wiki.Core.Infrastructure;
using Griffin.Wiki.Core.Users.DomainModels;

namespace Griffin.Wiki.Core.Templates.DomainModels
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
        /// Initializes a new instance of the <see cref="PageTemplate"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="content">The content.</param>
        public PageTemplate(string title, string content)
        {
            CreatedAt = DateTime.Now;
            CreatedBy = WikiContext.CurrentUser;
            Title = title;
            Content = content;
            UpdatedAt = DateTime.Now;
            UpdatedBy = WikiContext.CurrentUser;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageTemplate"/> class.
        /// </summary>
        /// <remarks>For nhibernate only</remarks>
        protected PageTemplate()
        {
        }

        /// <summary>
        /// Gets or sets database id
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        /// Gets or sets a title describing what the template is for
        /// </summary>
        public virtual string Title { get; protected set; }


        /// <summary>
        /// Gets or sets instructions intended purpose of the template (where to write what)
        /// </summary>
        public virtual string Instructions { get; protected set; }


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
        public virtual User UpdatedBy { get; protected set; }

        /// <summary>
        /// Gets or sets when the item was created
        /// </summary>
        public virtual DateTime CreatedAt { get; protected set; }

        /// <summary>
        /// Gets or sets who created the item
        /// </summary>
        public virtual User CreatedBy { get; protected set; }

        /// <summary>
        /// Update a template
        /// </summary>
        /// <param name="title">New title</param>
        /// <param name="content">Updated content (as entered by user, not parsing should have been done)</param>
        public virtual void Update(string title, string content)
        {
            if (title == null) throw new ArgumentNullException("title");
            if (content == null) throw new ArgumentNullException("content");

            Title = title;
            Content = content;
            UpdatedAt = DateTime.Now;
            UpdatedBy = WikiContext.CurrentUser;
        }
    }
}