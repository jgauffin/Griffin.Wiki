using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Griffin.Wiki.Core.Pages;

namespace Griffin.Wiki.WebClient.Areas.Wiki.Models.Page
{
    public class ShowViewModel
    {
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public PagePath PagePath { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<string> BackLinks { get; set; }

        public string TableOfContents { get; set; }

        public string Path { get; set; }
    }
}