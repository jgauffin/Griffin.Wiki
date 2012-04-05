using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Griffin.Wiki.Core.Pages;

namespace Griffin.Wiki.WebClient.Areas.Wiki.Models.Review
{
    public class ShowViewModel
    {
        public string CreatedBy { get; set; }
        public string Body { get; set; }
        public PagePath PagePath { get; set; }
        public int RevisionId { get; set; }
    }
}