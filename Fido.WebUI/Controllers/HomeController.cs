using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Fido.Core;
using Fido.Action;
using Fido.WebUI.Extensions;
using Fido.WebUI.Flash.Messages;

namespace Fido.WebUI.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return PartialView();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}
