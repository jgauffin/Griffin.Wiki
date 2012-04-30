using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Griffin.Logging;
using Griffin.Wiki.Core.Images.DomainModels;
using Griffin.Wiki.Core.Images.Repositories;
using Griffin.Wiki.Core.Pages;
using Griffin.Wiki.Mvc3.Areas.Wiki.Models.Image;

namespace Griffin.Wiki.Mvc3.Areas.Wiki.Controllers
{
    public class ImageController : BaseController
    {
        private readonly IImageRepository _repository;
        private ILogger _logger = LogManager.GetLogger<ImageController>();

        public ImageController(IImageRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index(PagePath id = null)
        {
            _logger.Warning("Get id " + id);
            var images = id == null
                             ? _repository.FindAll()
                             : _repository.FindForPage(id);


            if (Request.Headers["Accept"].Contains("json"))
            {
                var response = new
                                   {
                                       success = true,
                                       body = images.Select(CreateImageModel).ToList()
                                   };
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            return ViewOrPartial(new IndexViewModel
                                     {
                                         Images = images,
                                         PagePath = (id ?? new WikiRoot()).ToString()
                                     });
        }
        public ActionResult Fake()
        {
            return View();
        }
        public ActionResult Thumbnail(int id)
        {
            var wikiImage = _repository.Get(id);

            using (var stream = wikiImage.GetFileStream())
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(stream);

                // Prevent using images internal thumbnail
                image.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
                image.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);


                var newWidth = 128;
                var newHeight = 128;
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


                var thumbStream = new MemoryStream();
                thumbNail.Save(thumbStream, ImageFormat.Jpeg);
                thumbStream.Position = 0;
                return new FileStreamResult(thumbStream, "image/jpeg");
            }

        }


        public ActionResult View(int id)
        {
            var image = _repository.Get(id);
            return new FileStreamResult(image.GetFileStream(), image.ContentType);
        }

        [HttpPost]
        public ActionResult Upload(string pagePath, string title, HttpPostedFileBase imageFile)
        {
            if (string.IsNullOrEmpty(pagePath))
                pagePath = "/";

            var path = new PagePath(pagePath);
            object result = null;
            if (imageFile == null || imageFile.ContentLength == 0)
            {
                result = new
                             {
                                 success = false,
                                 body = "No file was uploaded."
                             };
            }
            else if (imageFile.ContentLength > 5000000)
            {
                result = new
                             {
                                 succcess = false,
                                 body = "Too large image, 5Mb is the limit."
                             };
            }
            else
            {
                var image = _repository.Create(path, Path.GetFileName(imageFile.FileName), title, imageFile.ContentType, imageFile.InputStream);
                result = new
                             {
                                 success = true,
                                 body = CreateImageModel(image)
                             };
            }


            return new WrappedJsonResult(result);
        }

        private object CreateImageModel(WikiImage image)
        {
            return new
                       {
                           url = Url.Action("Thumbnail", new { id = image.Id }),
                           id = image.Id,
                           filename = image.Filename
                       };
        }

        public class WrappedJsonResult : JsonResult
        {
            public WrappedJsonResult(object result)
            {
                Data = result;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                context.HttpContext.Response.Write("<html><body><textarea id=\"json-result\" name=\"json-result\">");
                base.ExecuteResult(context);
                context.HttpContext.Response.Write("</textarea></body></html>");
                context.HttpContext.Response.ContentType = "text/html";
            }

        }

    }
}
