using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Wiki.Core.Pages.Content.Services;

namespace Griffin.Wiki.Core.Pages
{
    /// <summary>
    /// Absolute path (from wiki root) to a wiki page
    /// </summary>
    public class PagePath : IEquatable<PagePath>, IPagePath
    {
        private readonly string _path;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagePath"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public PagePath(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (!path.StartsWith("/") || !path.EndsWith("/"))
                throw new ArgumentException("An wiki path should start with a slash and end with a slash");

            _path = path.ToLower();
        }

        /// <summary>
        /// Gets name of this page (last part of path)
        /// </summary>
        public string Name
        {
            get
            {
                var name = _path.TrimEnd('/');
                if (_path == "/")
                    return "";

                var pos = name.LastIndexOf('/');
                return name.Substring(pos + 1, 1).ToUpper() + name.Substring(pos + 2);
            }
        }

        /// <summary>
        /// Gets path to 
        /// </summary>
        public PagePath ParentPath
        {
            get
            {
                var pos = _path.TrimEnd('/').LastIndexOf('/');
                return pos == -1 ? new WikiRoot() : new PagePath(_path.Substring(0, pos + 1));
            }
        }

        /// <summary>
        /// Gets path parts in reverse order (most specific first)
        /// </summary>
        /// <returns>Path parts</returns>
        /// <example><code>
        /// path.GetReverseParts(); // []{"/some/path/in/wiki", "/some/path/in/", "/some/path/", "/some/", "/"}
        /// </code>
        /// </example>
        public IEnumerable<PagePath> GetReverseParts()
        {

            var pos = _path.LastIndexOf('/');
            while (pos > 0)
            {
                yield return new PagePath(_path.Substring(0, pos + 1));
                pos = _path.LastIndexOf('/', pos - 1);
            }
            yield return new PagePath("/");
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(PagePath other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return _path.Equals(other._path, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Create a child path
        /// </summary>
        /// <param name="childName">Name of child (alphanumeric)</param>
        /// <returns>Child path</returns>
        public PagePath CreateChildPath(string childName)
        {
            return new PagePath(string.Format("{0}{1}/", _path, childName));
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _path;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as PagePath;
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return _path.Equals(other._path, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return _path.GetHashCode();
        }

        /// <summary>
        /// Get path to all parents, starting with root
        /// </summary>
        /// <returns>One path per parent</returns>
        /// <example><code>
        /// // page = "/guidelines/patterns/singleton"
        /// page.GetPathForParents() //--> []{"/", "/guidelines/", "/guidelines/patterns/" };
        /// </code></example>
        public IEnumerable<PagePath> GetPathForParents()
        {
            var parts = _path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var path = "/";
            yield return new PagePath(path);
            for (int i = 0; i < parts.Length - 1; i++)
            {
                path += parts[i] + "/";
                yield return new PagePath(path);
            }
        }

        /// <summary>
        /// Get path relative to another path
        /// </summary>
        /// <param name="pagePath">Page to get a relative path to</param>
        /// <returns>Relative path</returns>
        /// <remarks>You are standing on the page that you invoke this method on.</remarks>
        public IPagePath GetPathRelativeTo(PagePath pagePath)
        {
            return new RelativePagePath(this, pagePath);
        }

    }

    /// <summary>
    /// Root page.
    /// </summary>
    public class WikiRoot : PagePath
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WikiRoot"/> class.
        /// </summary>
        public WikiRoot()
            : base("/")
        {

        }
    }

    public class RelativePagePath : IPagePath
{
        private readonly PagePath _sourcePath;
        private readonly PagePath _destinationPath;
        private string _path;

        public RelativePagePath(PagePath sourcePath, PagePath destinationPath)
        {
            _sourcePath = sourcePath;
            _destinationPath = destinationPath;

            var uri1 = new Uri("http://somejunk" + sourcePath);
            var uri2 = new Uri("http://somejunk" + destinationPath);
            _path = uri1.MakeRelativeUri(uri2).ToString().TrimEnd('/');
        }

        public RelativePagePath(PagePath sourcePath, string relativePath)
        {
            if (!relativePath.EndsWith("/"))
                relativePath += "/";

            var uri1 = new Uri("http://somejunk" + sourcePath);
            var uri2 = new Uri(uri1, relativePath);
            var absolute = uri2.AbsolutePath;

            _destinationPath = new PagePath(absolute);
            _path = relativePath.TrimEnd('/');
            _sourcePath = sourcePath;
        }


        private int GetCommonLength(PagePath source, PagePath dest)
        {
            var sourcePath = source.ToString();
            var destPath = dest.ToString();
            var min = Math.Min(sourcePath.Length, destPath.Length);

            for (var i = 0; i < min; i++)
            {
                if (sourcePath[i] != destPath[i])
                {
                    return i;
                }
            }

            return 1;
        }

        public override string ToString()
        {
            return _path;
        }

        public PagePath ToAbsolute()
        {
            return _destinationPath;
        }
}
}
