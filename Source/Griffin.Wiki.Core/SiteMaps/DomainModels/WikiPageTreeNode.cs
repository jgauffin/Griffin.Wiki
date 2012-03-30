using System;
using Griffin.Wiki.Core.Pages;
using Griffin.Wiki.Core.Pages.DomainModels;

namespace Griffin.Wiki.Core.SiteMaps.DomainModels
{
    /// <summary>
    /// 
    /// </summary>
    public class WikiPageTreeNode
    {
        private WikiPage _page;

        public WikiPageTreeNode()
        {
        }

        public WikiPageTreeNode(WikiPage page, WikiPageTreeNode parentNode)
        {
            if (page == null) throw new ArgumentNullException("page");
            if (parentNode == null)
            {
                Lineage = string.Format("/{0}/", page.Id);
                Titles = page.Title;
                Path = page.PagePath;
                Depth = 1;
            }
            else
            {
                Lineage = string.Format("{0}{1}/", parentNode.Lineage, page.Id);
                Titles = string.Format("{0}{{#}}{1}", parentNode.Page.Title, page.Title);
                Path = page.PagePath;
                Depth = parentNode.Depth + 1;
            }

            Page = page;
        }

        public virtual int PageId { get; protected set; }

        public virtual WikiPage Page
        {
            get { return _page; }
            protected set
            {
                _page = value;
                PageId = value.Id;
            }
        }

        public virtual string Lineage { get; protected set; }
        public virtual string Titles { get; set; }
        public virtual PagePath Path { get; protected set; }

        public virtual int Depth { get; protected set; }

        /// <summary>
        /// Get dash sperated ids for the parent document
        /// </summary>
        public virtual string ParentLinage
        {
            get
            {
                var pos = Lineage.TrimEnd('/').LastIndexOf('/');
                var result = Lineage.Substring(0, pos + 1);
                return result == "/" ? "" : result;
            }
        }

        /// <summary>
        /// Create a linked path to the page
        /// </summary>
        /// <param name="pageUri">Url used to show pages</param>
        /// <returns>HTML links separated by dashes</returns>
        /// <example>
        /// <code>
        /// var link = node.CreateLinkPath(Url.Action("Show", "Page")); //--> <![CDATA[<a href="/page/show/home">Home</a> / <a href="/page/show/users">Users</a>]]>
        /// </code>
        /// </example>
        public virtual string CreateLinkPath(string pageUri)
        {
            if (pageUri == null) throw new ArgumentNullException("pageUri");
            var titles = Titles.Split(new[] { "{#}" }, StringSplitOptions.RemoveEmptyEntries);
            var names = Path.ToString().Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (names.Length == 1)
            {
                return string.Format(@"<a href=""{0}{1}"">{1}</a>", names[0], titles[0]);
            }

            if (!pageUri.EndsWith("/"))
                pageUri += "/";

            var result = string.Format(@"<a href=""{0}"">Home</a> / ", pageUri);

            var path = "";
            for (var i = 0; i < names.Length; i++)
            {
                path += names[i] + "/";
                result += string.Format(@"<a href=""{0}{1}"">{2}</a> / ", pageUri, path, titles[i]);
            }

            return result == "" ? result : result.Remove(result.Length - 3, 3);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("#{0} {1}", PageId, Titles);
        }

        /// <summary>
        /// Create a link for th e
        /// </summary>
        /// <param name="pageUri">Uri used to generate a link</param>
        /// <returns>A HTML link</returns>
        /// <example>
        /// <code>
        /// var link = node.CreateLink(Url.Action("Show", "Page")); //--> <![CDATA[<a href="/page/show/users">Users</a>]]>
        /// </code>
        /// </example>
        public virtual string CreateLink(string pageUri)
        {
            if (pageUri == null) throw new ArgumentNullException("pageUri");

            if (!pageUri.EndsWith("/"))
                pageUri += "/";

            return string.Format(@"<a href=""{0}{1}"">{2}</a>", pageUri, Page.PagePath, Page.Title);
        }
    }
}