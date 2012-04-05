﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Griffin.Wiki.Core;

namespace Griffin.Wiki.WebClient.Infrastructure
{
    public class WikiUriHelper : IUriHelper
    {
        private UrlHelper _helper;

        public WikiUriHelper()
        {
            _helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public string GetWikiRoot()
        {

            return _helper.RouteUrl("Wiki");
        }

        public string CreateLinkFromRoute(object route)
        {
            return _helper.RouteUrl("WikiAdmin", route);
        }
    }
}