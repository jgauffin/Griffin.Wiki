using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Griffin.Wiki.Core.Pages;

namespace Griffin.Wiki.Mvc3.Areas.Wiki.Models.Review
{
    public class DenyViewModel
    {
        [Required]
        public PagePath Id { get; set; }

        [Required]
        public int Revision { get; set; }

        [Required]
        public string Reason { get; set; }
    }
}