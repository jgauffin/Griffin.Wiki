using Sogeti.Pattern.DomainEvents;

namespace Griffin.Wiki.Core.Pages.DomainModels.Events
{
    /// <summary>
    ///   A page has been updated.
    /// </summary>
    public class PageUpdated : IDomainEvent
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="PageUpdated" /> class.
        /// </summary>
        /// <param name="page"> The page. </param>
        public PageUpdated(WikiPage page)
        {
            Page = page;
        }

        /// <summary>
        ///   Gets the page that the event is for ;)
        /// </summary>
        public WikiPage Page { get; private set; }
    }
}