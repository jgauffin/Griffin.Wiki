using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Griffin.Wiki.WebClient.Models.Template
{
    public class CreateViewModel
    {
        [Required]
        public string TemplateTitle { get; set; }

        [Required]
        public string TemplateContent { get; set; }
    }
}