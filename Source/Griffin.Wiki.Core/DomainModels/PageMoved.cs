using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sogeti.Pattern.DomainEvents;

namespace Griffin.Wiki.Core.DomainModels
{
    public class PageMoved : IDomainEvent
    {
        public PageMoved(WikiPage subject, WikiPage oldParent)
        {
            
        }
    }
}
