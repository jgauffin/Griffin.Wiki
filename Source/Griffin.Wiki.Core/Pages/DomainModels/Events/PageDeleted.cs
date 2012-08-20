

namespace Griffin.Wiki.Core.Pages.DomainModels.Events
{
    /// <summary>
    ///   A page has been created.
    /// </summary>
    public class PageDeleted : IDomainEvent
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="PageDeleted" /> class.
        /// </summary>
        /// <param name="page"> The page. </param>
        public PageDeleted(WikiPage page)
        {
            Page = page;
        }

        /// <summary>
        ///   Gets the page that the event is for ;)
        /// </summary>
        public WikiPage Page { get; private set; }
    }
}