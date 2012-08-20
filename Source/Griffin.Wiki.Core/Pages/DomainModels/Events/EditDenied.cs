using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Griffin.Wiki.Core.Pages.DomainModels.Events
{
    public class EditDenied : IDomainEvent
    {
        public WikiPageRevision Revision { get; set; }
        public string Reason { get; set; }

        public EditDenied(WikiPageRevision revision, string reason)
        {
            Revision = revision;
            Reason = reason;
        }
    }
}
