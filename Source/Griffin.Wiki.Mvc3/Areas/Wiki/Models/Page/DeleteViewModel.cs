﻿using System.Collections.Generic;

namespace Griffin.Wiki.Mvc3.Areas.Wiki.Models.Page
{
    public class DeleteViewModel
    {
        public string PageName { get; set; }

        public string Title { get; set; }

        public List<string> Children { get; set; }
    }
}