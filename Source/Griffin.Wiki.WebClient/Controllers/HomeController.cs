using System.Web.Mvc;

namespace Griffin.Wiki.WebClient.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToRoute("Wiki", new {pagePath = "/"});
        }

        public ActionResult About()
        {
            return View();
        }
    }
}