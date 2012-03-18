using System;

namespace Griffin.Wiki.Core.Pages.DomainModels
{
    /// <summary>
    /// Link from one page to another.
    /// </summary>
    public class WikiPageLink
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WikiPageLink"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="linkedPage">The linked page.</param>
        public WikiPageLink(WikiPage page, WikiPage linkedPage)
        {
            if (page == null) throw new ArgumentNullException("page");
            if (linkedPage == null) throw new ArgumentNullException("linkedPage");
            Page = page;
            LinkedPage = linkedPage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WikiPageLink"/> class.
        /// </summary>
        protected WikiPageLink()
        {
        }

        /// <summary>
        /// Gets page that links to another page
        /// </summary>
        public virtual WikiPage Page { get; protected set; }

        /// <summary>
        /// Gets page that the current page links to
        /// </summary>
        public virtual WikiPage LinkedPage { get; protected set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as WikiPageLink;
            if (other == null)
                return false;

            return other.Page.Id == Page.Id && other.LinkedPage.Id == LinkedPage.Id;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return string.Format("{0}-{1}", Page.GetHashCode(), LinkedPage.GetHashCode()).GetHashCode();
        }
    }
}