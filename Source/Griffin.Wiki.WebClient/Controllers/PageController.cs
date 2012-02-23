using System.Web;
using System.Web.Mvc;
using Griffin.Wiki.Core.Repositories;

namespace Griffin.Wiki.WebClient.Controllers
{
    public class PageController : Controller
    {
        private readonly IPageRepository _repository;

        public PageController(IPageRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            return View();
        }

        protected override void HandleUnknownAction(string actionName)
        {
            base.HandleUnknownAction(actionName);
        }

        public ActionResult Details(string id)
        {
            var page = _repository.Get(id);
            if (page == null)
                throw new HttpException(404, "Page " + id + " do not exist.");

            return View(page);
        }
    }
}