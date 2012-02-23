using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectPortal.Core.Repositories.Documents
{
    public class PageDocument
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public string HtmlBody { get; set; }
        public string RawBody { get; set; }
        public PageDocumentUser CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public IList<PageLink> References { get; set; }
        public IList<PageLink> ReferencedBy { get; set; }
        public IList<HistoryItem> Revisions { get; set; }
    }
}
