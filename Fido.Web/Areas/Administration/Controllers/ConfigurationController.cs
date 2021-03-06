﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Fido.Core;
using Fido.ViewModel;
using Fido.ViewModel.Models.Administration;
using Fido.Web.Common;
using Fido.Web.Binders;

namespace Fido.Web.Areas.Administration.Controllers
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
