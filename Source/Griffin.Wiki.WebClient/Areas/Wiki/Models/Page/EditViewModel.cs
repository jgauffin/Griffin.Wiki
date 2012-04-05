using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Griffin.Wiki.Core.Pages;

namespace Griffin.Wiki.WebClient.Areas.Wiki.Models.Page
{
    public class EditViewModel
    {
        [Required]
        public PagePath Path { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [AllowHtml]
        public string Content { get; set; }

        [Display(Name="What have you changed?")]
        [Required]
        public string Comment { get; set; }
    }
}