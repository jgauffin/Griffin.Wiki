using System;

namespace Griffin.Wiki.Core.Pages.Content.Services
{
    public class WikiLink : IEquatable<WikiLink>
    {
        public PagePath PagePath { get; set; }
        public string Title { get; set; }
        public bool Exists { get; set; }

        #region IEquatable<WikiLink> Members

        public bool Equals(WikiLink other)
        {
            if (other == null)
                return false;
            return PagePath.Equals(other.PagePath);
        }

        #endregion
    }
}