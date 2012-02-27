using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Griffin.Wiki.WebClient.Models.Page
{
    public class ShowViewModel
    {
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string PageName { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}