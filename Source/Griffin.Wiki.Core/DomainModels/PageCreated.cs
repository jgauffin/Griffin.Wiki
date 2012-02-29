using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sogeti.Pattern.DomainEvents;

namespace Griffin.Wiki.Core.DomainModels
{
    /// <summary>
    /// A page has been created.
    /// </summary>
    public class PageCreated : IDomainEvent 
    {
        public WikiPage Page { get; private set; }

        public PageCreated(WikiPage page)
        {
            Page = page;
        }
    }
}
