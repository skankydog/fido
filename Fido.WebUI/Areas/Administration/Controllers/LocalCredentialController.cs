﻿using System;
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
    public class LocalCredentialController : BaseController
    {
        public ActionResult Update(Guid Id)
        {
            return Dispatcher.ReturnLoadedModel<LocalCredentialAdminVM>(
                Id: Id,
                Result: m => PartialView(m));
        }

        [HttpPost]
        public ActionResult Update(LocalCredentialAdminVM Model)
        {
            return Dispatcher.SavePostedModel(
                DataModel: Model,
                SuccessResult: m => ModalRedirectToLocal(Url.Action("Update", "User", new { Model.Id })),
                NonsuccessResult: m => ModalRedirectToLocal(Url.Action("Update", "User", new { Model.Id })));
        }

        public ActionResult Delete(Guid Id)
        {
            return Dispatcher.ReturnLoadedModel<LocalCredentialAdminVM>(
                Id: Id,
                Result: m => PartialView(m));
        }

        [HttpPost]
        public ActionResult Delete(LocalCredentialAdminVM Model)
        {
            return Dispatcher.DeletePostedModel(
                DataModel: Model,
                Result: m => ModalRedirectToLocal(Url.Action("Update", "User", new { Model.Id })));
        }
    }
}
