using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Griffin.Wiki.WebClient.Models.Page
{
    public class DeleteViewModel
    {
        public string PageName { get; set; }

        public string Title { get; set; }

        public List<string> Children { get; set; }
    }
}