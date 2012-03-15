﻿using System.Web.Mvc;
using Griffin.Wiki.Core.Services;
using Griffin.Wiki.WebClient.Infrastructure.Helpers;

namespace Griffin.Wiki.WebClient.Areas.Wiki.Controllers
{
    public class SiteMapController : Controller
    {
        private readonly SiteMapService _siteMapService;

        public SiteMapController(SiteMapService siteMapService)
        {
            _siteMapService = siteMapService;
        }

        public ActionResult Index()
        {
            var map =_siteMapService.Get(Url.WikiRoot());

            return View(map);
        }

        public ActionResult Partial(string pageName)
        {
            var map = _siteMapService.GetPartial(pageName, Url.WikiRoot());

            return View("Index", map);
        }
    }
}