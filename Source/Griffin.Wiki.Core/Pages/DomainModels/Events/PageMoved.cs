using Sogeti.Pattern.DomainEvents;

namespace Griffin.Wiki.Core.Pages.DomainModels.Events
{
    public class PageMoved : IDomainEvent
    {
        public PageMoved(WikiPage subject, WikiPage oldParent)
        {
        }
    }
}