using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Griffin.Wiki.Core.Pages;

namespace Griffin.Wiki.Mvc3.Areas.Wiki.Models.Review
{
    public class IndexViewModel
    {
        public IEnumerable<IndexViewModelItem> Items { get; set; }
    }

    public class IndexViewModelItem
    {
        public int RevisionId { get; set; }
        public PagePath Path { get; set; }
        public DateTime CreatedAt { get; set; }
        public string EditedBy { get; set; }
    }
}