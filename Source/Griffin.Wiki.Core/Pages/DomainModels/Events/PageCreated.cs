using Sogeti.Pattern.DomainEvents;

namespace Griffin.Wiki.Core.Pages.DomainModels.Events
{
    /// <summary>
    ///   A page has been created.
    /// </summary>
    public class PageCreated : IDomainEvent
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="PageCreated" /> class.
        /// </summary>
        /// <param name="page"> The page. </param>
        public PageCreated(WikiPage page)
        {
            Page = page;
        }

        /// <summary>
        ///   Gets the page that the event is for ;)
        /// </summary>
        public WikiPage Page { get; private set; }
    }
}