using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Griffin.Wiki.WebClient.Models.Page
{
    public class CreateViewModel
    {
        public string PageName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}