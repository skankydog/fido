using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Fido.Core;
using Fido.WebUI.Common;
using Fido.WebUI.Filters;
using Fido.WebUI.Flash;
using Fido.ViewModel;

namespace Fido.WebUI.Common
{
    [AntiForgeryTokenFilter]
    public abstract class BaseController : Controller, IFeedbackAPI, IAuthenticationAPI, IModelAPI
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected IDispatcher<ActionResult> Dispatcher;
        protected IPusher Flash { get; set; }

        protected BaseController()
        {
            Dispatcher = ViewModelFactory.CreateDispatcher<ActionResult>(
                this, this, this,
                AuthoriseResult: () => new HttpUnauthorizedResult(),
                PasswordResetResult: (m) => RedirectToAction("Update", "Password", new { Area = "Account" }),
                DefaultErrorResult: (m) => RedirectToAction("Create", "Login", new { Area = "Authentication" })); // TO DO: Change this to a dedicated "error page" instead
            Flash = new Flasher();
        }

        //private bool IsAjax(ExceptionContext FilterContext)
        //{
        //    return FilterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        //}

        //protected override void OnException(ExceptionContext FilterContext)
        //{
        //    using (new FunctionLogger(Log))
        //    {
        //        Log.Error("Controller: " + (string)FilterContext.RouteData.Values["controller"]);
        //        Log.Error("Action: " + (string)FilterContext.RouteData.Values["action"]);
        //        Log.Error("Exception: " + FilterContext.Exception.ToString());

        //        // Check the exception type that has been throw. By default, we want to display the general error
        //        // page, but if any of the know exceptions are throwm, we can display a much more applicable
        //        // view...
        //        // string ErrorViewName = FilterContext.Exception.ToString().Split('.').Last();
        //        FilterContext.Result = new ViewResult { ViewName = "Error-General" };
        //        if (FilterContext.Exception is EmailAddressDuplicationException) FilterContext.Result = new ViewResult { ViewName = "Error-EmailAddressDuplication" };
        //        if (FilterContext.Exception is ConcurrencyException) FilterContext.Result = new ViewResult { ViewName = "Error-Concurrency" };
        //        if (FilterContext.Exception is UniqueFieldException) FilterContext.Result = new ViewResult { ViewName = "Error-UniqueField" };

        //        FilterContext.ExceptionHandled = true;
        //    }
        //}

        protected ActionResult RedirectToLocal(string ReturnUrl)
        {
            if (Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Area = string.Empty });
            }
        }

        protected ActionResult ModalRedirectToLocal(string ReturnUrl)
        {
            if (!Url.IsLocalUrl(ReturnUrl))
            {
                Log.WarnFormat("Attempt to redirect to non-local location: {0}", ReturnUrl);
                return RedirectToAction("Index", "Home");
            }

            Log.InfoFormat("RedirectorModel.Location={0}", ReturnUrl);

            return RedirectToAction("ModalRedirectToLocal", "Home",
                        new
                        {
                            Area = string.Empty, // The redirector is in the base area
                            Location = ReturnUrl // The URL to redirect to
                        });
        }

        #region IFeedbackAPI Implementation
        public void DisplayInfo(string Message)
        {
            Flash.Info(Message);
        }

        public void DisplaySuccess(string Message)
        {
            Flash.Success(Message);
        }

        public void DisplayWarning(string Message)
        {
            Flash.Warning(Message);
        }

        public void DisplayError(string Message)
        {
            Flash.Error(Message);
        }
        #endregion

        #region IAuthenticationAPI Implementation
        public void SignIn(Guid UserId, string Fullname, bool RememberMe)
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            List<Claim> Claims = new List<Claim>
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", Fullname),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", UserId.ToString()), // User.Id from my database
                new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "MyApplication"),
            };

            ClaimsIdentity Identity = new System.Security.Claims.ClaimsIdentity(Claims, DefaultAuthenticationTypes.ApplicationCookie, ClaimTypes.Name, ClaimTypes.Role);
            HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties() { IsPersistent = RememberMe }, Identity);
        }

        public void SignOut()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
        }

        public bool Authenticated { get { return User.Identity.IsAuthenticated; } }

        public string LoggedInCredentialState
        {
            get
            {
                if (!Authenticated || Request.Cookies["loggedincredentialstate"] == null)
                    return "None";

                return Request.Cookies["loggedincredentialstate"].Value;
            }

            set
            {
                Response.Cookies["loggedincredentialstate"].Value = value;
            }
        }

        public Guid AuthenticatedId
        {
            get
            {
                if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                    return Guid.Empty; // allows redirect to login to occur

                return User.Identity.GetUserId().ToGuid();
            }
        }
        #endregion

        #region IModelAPI Implementation
        public void PropertyError(string Field, string Message)
        {
            ModelState.AddModelError(Field, Message);
        }

        public void ModelError(string Message)
        {
            ModelState.AddModelError("", Message);
        }

        public bool ModelStateIsValid()
        {
            return ModelState.IsValid;
        }

        public Mode Mode
        {
            get
            {
                string ModeStr = System.Configuration.ConfigurationManager.AppSettings["UI-Mode"];

                if (ModeStr == "Production")
                    return Mode.Production;

                if (ModeStr == "Testing")
                    return Mode.Test;

                return Mode.Development;
            }
        }
        #endregion
    }
}
