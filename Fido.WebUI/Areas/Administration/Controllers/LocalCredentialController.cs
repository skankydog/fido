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
    public class LocalCredentialController : BaseController
    {
        public ActionResult Create()
        {
            return Dispatcher.CreateView(
                DataModel: new LocalCredentialModel(),
                Result: m => PartialView(m));
        }

        [HttpPost]
        public ActionResult Create(LocalCredentialModel Model)
        {
            return Dispatcher.Create(
                DataModel: Model,
                SuccessResult: () => ModalRedirectToLocal(Url.Action("Update", "User", Model.Id)),
                NonsuccessResult: m => ModalRedirectToLocal(Url.Action("Update", "User", Model.Id)));
        }

        public ActionResult Delete(Guid Id)
        {
            return Dispatcher.DeleteView<LocalCredentialModel>(
                Id: Id,
                Result: m => PartialView(m));
        }

        [HttpPost]
        public ActionResult Delete(LocalCredentialModel Model)
        {
            return Dispatcher.Delete_<LocalCredentialModel>(
                DataModel: Model,
                Result: () => ModalRedirectToLocal(Url.Action("Update", "User", Model.Id)));
        }
    }
}
