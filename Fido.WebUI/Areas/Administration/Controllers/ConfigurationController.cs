using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Fido.Core;
using Fido.Action;
using Fido.Action.Models.Administration;
using Fido.WebUI.Common;
using Fido.WebUI.Binders;

namespace Fido.WebUI.Areas.Administration.Controllers
{
    public class ConfigurationController : BaseController
    {
        public ActionResult Update()
        {
            return Dispatcher.Load<Configuration>(
                Id: Guid.Empty,
                Result: m => View(m));
        }

        [HttpPost]
        public ActionResult Update(Configuration Model)
        {
            return Dispatcher.Update(
                DataModel: Model,
                SuccessResult: m => RedirectToAction("Index", "Configuration"),
                InvalidResult: m => View(m));
        }
    }
}
