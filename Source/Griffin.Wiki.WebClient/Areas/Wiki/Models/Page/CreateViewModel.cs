using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Griffin.Wiki.WebClient.Areas.Wiki.Models.Page
{
    public class CreateViewModel
    {
        [Required]
        public string PageName { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public string ParentName { get; set; }

        [Required]
        public int ParentId { get; set; }

        
        public int TemplateId { get; set; }

        public IEnumerable<SelectListItem> Templates { get; set; }
    }
}