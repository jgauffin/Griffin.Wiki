using System.Web.Mvc;
using Griffin.Wiki.Core.Infrastructure.Authorization.Mvc;
using Griffin.Wiki.Core.Pages;
using Griffin.Wiki.Core.Pages.Repositories;
using Griffin.Wiki.Core.Templates.Repositories;
using Griffin.Wiki.WebClient.Areas.Wiki.Models;
using Griffin.Wiki.WebClient.Areas.Wiki.Models.Template;
using Griffin.Wiki.WebClient.Controllers;

namespace Griffin.Wiki.WebClient.Areas.Wiki.Controllers
{
    [WikiAuthorize]
    public class TemplateController : BaseController
    {
        private readonly ITemplateRepository _repository;
        private readonly IPageRepository _pageRepository;

        public TemplateController(ITemplateRepository repository, IPageRepository pageRepository)
        {
            _repository = repository;
            _pageRepository = pageRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Create()
        {
            var model = new CreateViewModel
            {
                TemplateInstructions =
                    @"Describe how the template should be used.

* For instance, should any of the headings be repeated? 
* Can any of the headings be replaced with own names? 
* Can any of the headings be removed?"

            };

            return ViewOrPartial(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Page id</param>
        /// <returns></returns>
        public ActionResult CreateFor(string id)
        {
            var path = new PagePath(id);
            var page = _pageRepository.Get(path);
            var model = new CreateViewModel
                            {
                                PagePath = path.ToString(),
                                TemplateTitle = "Template for " + page.Title,
                                TemplateInstructions =
                                    @"Describe how the template should be used.

* For instance, should any of the headings be repeated? 
* Can any of the headings be replaced with own names? 
* Can any of the headings be removed?"

                            };

            return View("Create", model);
        }

        [HttpPost, Transactional2, Authorize]
        public ActionResult Create(CreateViewModel model)
        {
            var template = _repository.Create(model.TemplateTitle, model.TemplateContent);

            if (!string.IsNullOrEmpty(model.PagePath))
            {
                var page = _pageRepository.Get(new PagePath(model.PagePath));
                page.ChildTemplate = template;
                _pageRepository.Save(page);
            }

            return Json(new JsonResponse<dynamic>(new
                                                      {
                                                          Key = template.Id,
                                                          Label = model.TemplateTitle
                                                      }));
        }

    }
}
