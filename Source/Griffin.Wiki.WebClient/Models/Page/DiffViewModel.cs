using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Griffin.Wiki.WebClient.Models.Page
{
    public class DiffViewModel
    {
        public string PageName { get; set; }
        public IEnumerable<DiffViewModelItem> Revisions { get; set; }
    }

    public class DiffViewModelItem
    {
        public int RevisionId { get; set; }
        public string UserDisplayName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}