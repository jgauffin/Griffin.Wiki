using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Web.Mvc;
using Griffin.Wiki.Core.Pages.Services;

namespace Griffin.Wiki.Mvc3.Areas.Wiki.Controllers
{
    public class PdfController : BaseController
    {
        private readonly OneDocService _oneDocService;

        public PdfController(OneDocService oneDocService)
        {
            _oneDocService = oneDocService;
        }

        public ActionResult Index()
        {
            Response.AddHeader("Content-Disposition", "attachment;filename=wiki.pdf");
            var ms = new MemoryStream();
            _oneDocService.GeneratePDF(HostingEnvironment.MapPath("~/App_Data/"), ms);
            ms.Position = 0;
            return new FileStreamResult(ms, "application/pdf");
        }
    }
}
