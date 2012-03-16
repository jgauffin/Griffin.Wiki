using System.Web.Mvc;
using Griffin.Wiki.Core.Repositories;
using Griffin.Wiki.WebClient.Areas.Wiki.Models;
using Griffin.Wiki.WebClient.Areas.Wiki.Models.Template;
using Griffin.Wiki.WebClient.Controllers;
using Griffin.Wiki.WebClient.Models;

namespace Griffin.Wiki.WebClient.Areas.Wiki.Controllers
{
    [WikiAuthorize]
    public class TemplateController : BaseController
    {
        private readonly TemplateRepository _repository;

        public TemplateController(TemplateRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Create()
        {
            return ViewOrPartial();
        }

        [HttpPost, Transactional2, Authorize]
        public ActionResult Create(CreateViewModel model)
        {
            var template = _repository.Create(model.TemplateTitle, model.TemplateContent);
            return Json(new JsonResponse<dynamic>(new
                                                      {
                                                          Key = template.Id,
                                                          Label = model.TemplateTitle
                                                      }));
        }

    }
}
