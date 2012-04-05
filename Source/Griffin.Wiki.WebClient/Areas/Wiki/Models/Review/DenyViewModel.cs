using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Griffin.Wiki.Core.Pages;

namespace Griffin.Wiki.WebClient.Areas.Wiki.Models.Review
{
    public class DenyViewModel
    {
        [Required]
        public PagePath PagePath { get; set; }
        [Required]
        public int RevisionId { get; set; }

        [Required]
        public string Reason { get; set; }
    }
}