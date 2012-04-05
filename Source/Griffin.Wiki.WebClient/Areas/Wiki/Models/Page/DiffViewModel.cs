using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Griffin.Wiki.Core.Pages;

namespace Griffin.Wiki.WebClient.Areas.Wiki.Models.Page
{
    public class DiffViewModel
    {
        public PagePath Path { get; set; }
        public IEnumerable<DiffViewModelItem> Revisions { get; set; }
    }

    public class DiffViewModelItem
    {
        public int RevisionId { get; set; }
        public string UserDisplayName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Comment { get; set; }
    }
}