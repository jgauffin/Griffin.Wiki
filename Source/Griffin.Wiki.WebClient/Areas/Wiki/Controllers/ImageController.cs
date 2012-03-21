using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
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

        public ActionResult Thumbnail(int id)
        {
            var wikiImage = _repository.Get(id);

            var ms = new MemoryStream(wikiImage.Body);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);

            // Prevent using images internal thumbnail
            image.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            image.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);


            var newWidth = 64;
            var newHeight = 64;
            if (image.Height > image.Width)
            {
                double ratio = newHeight / (double)image.Height;
                newWidth = (int)(image.Width * ratio);
            }
            else
            {
                double ratio = newWidth / (double)image.Width;
                newHeight = (int)(image.Height * ratio);
            }

            System.Drawing.Image thumbNail = image.GetThumbnailImage(newWidth, newHeight, null, IntPtr.Zero);
            image.Dispose();
            ms.Dispose();

            ms = new MemoryStream();
            thumbNail.Save(ms, ImageFormat.Jpeg);
            ms.Position = 0;
            return new FileStreamResult(ms, "image/jpeg");
        }


        public ActionResult View(int id)
        {
            var image = _repository.Get(id);
            var ms = new MemoryStream(image.Body);
            return new FileContentResult(image.Body, image.ContentType);
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
                                               url = Url.Action("View", new { id = image.Id })
                                           }
                            });
        }

    }
}
