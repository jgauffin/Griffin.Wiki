using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Griffin.Wiki.WebClient.Areas.Wiki.Models.Page
{
    public class EditViewModel
    {
        [Required]
        public string PageName { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Display(Name="What have you changed?")]
        [Required]
        public string Comment { get; set; }
    }
}