using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Griffin.Wiki.Core.Pages;
using Griffin.Wiki.Core.Pages.Repositories;
using Griffin.Wiki.WebClient.Areas.Wiki.Models.Review;
using Griffin.Wiki.WebClient.Infrastructure.Helpers;

namespace Griffin.Wiki.WebClient.Areas.Wiki.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IPageRepository _repository;

        public ReviewController(IPageRepository repository)
        {
            _repository = repository;
        }


        public ActionResult Index()
        {
            var revisions = _repository.GetRevisionsToApprove();
            var model = new IndexViewModel();
            model.Items = revisions.Select(x => new IndexViewModelItem
                                                    {
                                                        CreatedAt = x.CreatedAt,
                                                        EditedBy = x.CreatedBy.DisplayName,
                                                        Path = x.Page.PagePath,
                                                        RevisionId = x.Id
                                                    });

            return View(model);
        }

        public ActionResult Show(PagePath id, int revision)
        {
            var page = _repository.Get(id);
            var subject = page.Revisions.Single(x => x.Id == revision);


            return View(new ShowViewModel
                            {
                                Body = subject.HtmlBody,
                                CreatedBy = subject.CreatedBy.DisplayName,
                                PagePath = page.PagePath,
                                RevisionId = revision
                            });
        }

        [HttpPost,Transactional2]
        public ActionResult Approve(PagePath id, int revisionId)
        {
            var page = _repository.Get(id);
            var subject = page.Revisions.Single(x => x.Id == revisionId);
            subject.Approve();
            return this.RedirectToWikiPage(id);
        }

        [HttpPost, Transactional2]
        public ActionResult Deny(DenyViewModel model)
        {
            var page = _repository.Get(model.Id);
            var subject = page.Revisions.Single(x => x.Id == model.RevisionId);
            subject.Deny(model.Reason);
            return this.RedirectToWikiPage(model.Id);
        }

        [HttpPost, Transactional2]
        public ActionResult Improve(PagePath id, int revisionId)
        {
            var page = _repository.Get(id);
            var subject = page.Revisions.Single(x => x.Id == revisionId);
            subject.ApproveButWillImprove();
            return RedirectToRoute("WikiAdmin", new { controller = "Page", action = "Edit", id = id.ToString() });
        }


    }
}
