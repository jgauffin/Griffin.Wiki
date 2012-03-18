using System;
using System.Collections.Generic;

namespace Griffin.Wiki.Core.DomainModels
{
    /// <summary>
    /// A node in the site map
    /// </summary>
    public class SiteMapNode
    {
        private readonly List<SiteMapNode> _children = new List<SiteMapNode>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteMapNode"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="link">HTML link to the current page</param>
        /// <param name="path">path (links) to the current page.</param>
        public SiteMapNode(string title, string link, string path)
        {
            if (title == null) throw new ArgumentNullException("title");
            if (path == null) throw new ArgumentNullException("path");

            Title = title;
            Link = link;
            Path = path;
        }

        /// <summary>
        /// Gets title of the page
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets HTML link
        /// </summary>
        public string Link { get; private set; }

        /// <summary>
        /// Gets path (links) to the current page
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Get a collection of all children
        /// </summary>
        public IEnumerable<SiteMapNode> Children
        {
            get { return _children; }
        }

        /// <summary>
        /// Add a child
        /// </summary>
        /// <param name="child">Child to add</param>
        public void AddChild(SiteMapNode child)
        {
            if (child == null) throw new ArgumentNullException("child");
            _children.Add(child);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Path;
        }
    }
}