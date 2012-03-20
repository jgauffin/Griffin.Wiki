using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Griffin.Wiki.Core.Images.DomainModels;
using Griffin.Wiki.Core.Images.Repositories;

namespace Griffin.Wiki.WebClient.Areas.Wiki.Controllers
{
    public class ImageController : Controller
    {
        private readonly ImageRepository _repository;

        public ImageController(ImageRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index(string pageName = "")
        {
            var images = pageName == ""
                             ? _repository.FindAll()
                             : _repository.FindForPage(pageName);

            return View(images);
        }

        public ActionResult View(int id)
        {
            var image = _repository.Get(id);
            var ms = new MemoryStream(image.Body);
            return new FileStreamResult(ms, image.ContentType);
        }

        [HttpPost]
        public ActionResult UploadImage(string pageName, HttpPostedFileBase imageFile)
        {
            if (imageFile == null || imageFile.ContentLength == 0)
            {
                return Json(new
                                {
                                    success = false,
                                    body = "No file was uploaded."
                                });
            }
            if (imageFile.ContentLength > 5000000)
            {
                return Json(new
                                {
                                    succcess = false,
                                    body = "Too large image, 5Mb is the limit."
                                });
            }

            var image = _repository.Create(pageName, imageFile.FileName);
            image.SetFile(imageFile.ContentType, imageFile.InputStream);

            return Json(new
                            {
                                success = true,
                                body = new
                                           {
                                               url = Url.Action("View", new {id = image.Id})
                                           }
                            });
        }

    }
}
