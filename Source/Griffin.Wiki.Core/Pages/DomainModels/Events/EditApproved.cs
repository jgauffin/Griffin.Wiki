using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Griffin.Wiki.Core.Pages.DomainModels.Events
{
    public class EditApproved : IDomainEvent
    {
        public WikiPageRevision Revision { get; set; }

        public EditApproved(WikiPageRevision revision)
        {
            if (revision == null) throw new ArgumentNullException("revision");
            Revision = revision;
        }
    }
}
