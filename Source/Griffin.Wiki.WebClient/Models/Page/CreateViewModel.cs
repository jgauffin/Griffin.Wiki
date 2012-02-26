using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Griffin.Wiki.WebClient.Models.Page
{
    public class CreateViewModel
    {
        [Required]
        public string PageName { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
    }
}