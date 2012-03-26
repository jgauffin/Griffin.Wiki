using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Wiki.Core.Pages
{
    /// <summary>
    /// Absolute path (from wiki root) to a wiki page
    /// </summary>
    public class PagePath : IEquatable<PagePath>
    {
        private readonly string _path;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagePath"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public PagePath(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (!path.StartsWith("/") && !path.EndsWith("/"))
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

        public PagePath ParentPath
        {
            get
            {
                var pos = _path.TrimEnd('/').LastIndexOf('/');
                return pos == -1 ? new WikiRoot() : new PagePath(_path.Substring(0, pos + 1));
            }
        }

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

        public IEnumerable<PagePath> GetPaths()
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
}
