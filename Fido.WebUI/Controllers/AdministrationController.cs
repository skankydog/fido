using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Fido.Core;
using Fido.Action.Models;

namespace Fido.WebUI.Controllers
{
    public class AdministrationController : BaseController
    {
        #region Users
        public ActionResult Users(int Page = 0)
        {
            return Dispatcher.Read<UsersModel>(
                Id: AuthenticatedId,
                Page: Page,
                SuccessUI: m => Json(m, JsonRequestBehavior.AllowGet));
        }
        #endregion
    }
}
