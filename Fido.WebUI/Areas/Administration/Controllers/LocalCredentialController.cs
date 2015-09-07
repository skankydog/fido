//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Mvc;
//using Microsoft.AspNet.Identity;
//using Microsoft.Owin.Security;
//using Fido.Core;
//using Fido.Action.Models;
//using Fido.WebUI.Common;

//namespace Fido.WebUI.Areas.Administration.Controllers
//{
//    public class LocalCredentialController : BaseController
//    {
//        public ActionResult Create()
//        {
//            return Dispatcher.View<LocalCredentialModel>(View);
//        }

//        [HttpPost]
//        public ActionResult Create(LocalCredentialModel Model)
//        {
//            return Dispatcher.Write(
//                Model: Model,
//                Any: m => RedirectToAction("Update", "User", Model.Id));
//        }

//        public ActionResult Update(Guid Id)
//        {
//            return Dispatcher.Read<LocalCredentialModel>(
//                Id: Id,
//                Success: m => View(m));
//        }

//        [HttpPost]
//        public ActionResult Update(LocalCredentialModel Model)
//        {
//            return Dispatcher.Write(
//                Model: Model,
//                Success: m => RedirectToAction("Update", "User", Model.Id),
//                Invalid: m => View(m),
//                Failure: m => View(m));
//        }

//        public ActionResult DeleteConfirmation(Guid Id)
//        {
//            return Dispatcher.Read<LocalCredentialModel>(
//                Id: Id,
//                Success: m => View(m));
//        }

//        [HttpPost]
//        public ActionResult Delete(Guid Id)
//        {
//            return Dispatcher.Delete_<LocalCredentialModel>(
//                Id: Id,
//                Any: () => RedirectToAction("Update", "User", Id));
//        }
//    }
//}
