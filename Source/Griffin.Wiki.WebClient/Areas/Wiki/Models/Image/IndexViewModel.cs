﻿using System.Collections.Generic;
using Griffin.Wiki.Core.Images.DomainModels;
using Griffin.Wiki.Core.Pages.DomainModels;

namespace Griffin.Wiki.WebClient.Areas.Wiki.Models.Image
{
    public class IndexViewModel
    {
        public string PageName { get; set; }
        public IEnumerable<WikiImage> Images { get; set; }

    }
}