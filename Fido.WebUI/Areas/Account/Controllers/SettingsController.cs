﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Fido.Core;
using Fido.Action.Models;
using Fido.WebUI.Common;

namespace Fido.WebUI.Areas.Account.Controllers
{
    public class SettingsController : BaseController
    {
        public ActionResult Index()
        {
            return Dispatcher.ReturnLoadedModel<Settings>(
                Id: AuthenticatedId,
                Result: m => View(m));
        }
    }
}