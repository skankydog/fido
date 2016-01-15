using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Fido.Core;
using Fido.Action;
using Fido.Action.Models;
using Fido.WebUI.Common;
using Fido.WebUI.Binders;

namespace Fido.WebUI.Areas.Administration.Controllers
{
    public class AdHomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return PartialView();
        }
    }
}
