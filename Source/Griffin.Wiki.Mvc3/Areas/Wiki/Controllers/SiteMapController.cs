using System.Web.Mvc;
using Griffin.Wiki.Core.Infrastructure.Authorization.Mvc;
using Griffin.Wiki.Core.Pages;
using Griffin.Wiki.Core.SiteMaps.Services;
using Griffin.Wiki.Mvc3.Helpers;

namespace Griffin.Wiki.Mvc3.Areas.Wiki.Controllers
{
    [WikiAuthorize]
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

            return PartialView(map);
        }

        public ActionResult Partial(PagePath pagePath)
        {
            var map = _siteMapService.GetPartial(pagePath, Url.WikiRoot());

            return PartialView(map);
        }
    }
}