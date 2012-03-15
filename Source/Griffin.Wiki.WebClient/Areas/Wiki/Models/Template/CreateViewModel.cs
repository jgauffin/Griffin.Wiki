﻿using System.ComponentModel.DataAnnotations;

namespace Griffin.Wiki.WebClient.Areas.Wiki.Models.Template
{
    public class CreateViewModel
    {
        [Required]
        public string TemplateTitle { get; set; }

        [Required]
        public string TemplateContent { get; set; }
    }
}