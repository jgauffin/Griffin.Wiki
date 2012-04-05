using System;
using System.Collections.Generic;
using Griffin.Wiki.Core.Pages;

namespace Griffin.Wiki.Core.SiteMaps.DomainModels
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
        /// <param name="path">Where the page is located.</param>
        /// <param name="pathLinks">HTML links (on per each step in the path) </param>
        public SiteMapNode(string title, PagePath path, string link, string pathLinks)
        {
            if (title == null) throw new ArgumentNullException("title");
            if (path == null) throw new ArgumentNullException("path");

            Title = title;
            Link = link;
            PathLinks = pathLinks;
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

        public string PathLinks { get; set; }

        /// <summary>
        /// Gets path (links) to the current page
        /// </summary>
        public PagePath Path { get; private set; }

        /// <summary>
        /// Gets or sets of the node is the current one.
        /// </summary>
        public bool IsCurrent { get; set; }

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
            return Path.ToString();
        }
    }
}