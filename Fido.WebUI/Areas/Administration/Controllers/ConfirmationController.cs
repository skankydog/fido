using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Fido.Core;
using Fido.ViewModel;
using Fido.ViewModel.Models.Administration;
using Fido.WebUI.Common;
using Fido.WebUI.Binders;

namespace Fido.WebUI.Areas.Administration.Controllers
{
    public class ConfirmationController : BaseController
    {
        public ActionResult Index(Guid UserId)
        {
            return Dispatcher.Index<ConfirmationIndex>(
                Id: UserId,
                Result: m => View(m));
        }

        public ActionResult List(ListOptions ListOptions)
        {
            return Dispatcher.List<ConfirmationList>(
                IndexOptions: ListOptions,
                Result: m => Json(m, JsonRequestBehavior.AllowGet));
        }

        public ActionResult Delete(Guid Id)
        {
            return Dispatcher.Load<Confirmation>(
                Id: Id,
                Result: m => PartialView(m));
        }

        [HttpPost]
        public ActionResult Delete(Confirmation Model)
        {
            return Dispatcher.DeleteIt<Confirmation>(
                DataModel: Model,
                Result: m => ModalRedirectToLocal(Url.Action("Index", "Confirmation", new { UserId = Model.UserId })));
        }
    }
}
