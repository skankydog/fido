﻿//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Mvc;
//using Microsoft.AspNet.Identity;
//using Microsoft.Owin.Security;
//using Fido.Core;
//using Fido.Action;
//using Fido.ViewModel.Models.Administration;
//using Fido.Web.Common;
//using Fido.Web.Binders;

//namespace Fido.Web.Areas.Administration.Controllers
//{
//    public class LocalCredentialController : BaseController
//    {
//        public ActionResult Update(Guid Id)
//        {
//            return Dispatcher.Update<LocalCredential>(
//                Id: Id,
//                Result: m => PartialView(m));
//        }

//        [HttpPost]
//        public ActionResult Update(LocalCredential Model)
//        {
//            return Dispatcher.Update(
//                DataModel: Model,
//                Result: m => ModalRedirectToLocal(Url.Action("Update", "User", new { Model.Id })));
//        }

//        public ActionResult Delete(Guid Id)
//        {
//            return Dispatcher.Update<LocalCredential>(
//                Id: Id,
//                Result: m => PartialView(m));
//        }

//        [HttpPost]
//        public ActionResult Delete(LocalCredential Model)
//        {
//            return Dispatcher.Delete(
//                DataModel: Model,
//                Result: m => ModalRedirectToLocal(Url.Action("Update", "User", new { Model.Id })));
//        }
//    }
//}
