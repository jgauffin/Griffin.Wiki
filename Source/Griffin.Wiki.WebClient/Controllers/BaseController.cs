using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Griffin.Logging;

namespace Griffin.Wiki.WebClient.Controllers
{
    public class BaseController : Controller
    {
        private readonly ILogger _logger;

        protected BaseController()
        {
            _logger = LogManager.GetLogger(GetType());
        }

        protected ILogger Logger
        {
            get { return _logger; }
        }
    }
}
