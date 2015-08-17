using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Fido.WebUI.Common
{
    internal class ChallengeResult : HttpUnauthorizedResult
    {
        public static string XsrfKey = "XsrfId";

        public ChallengeResult(string Provider, string RedirectUri)
            : this(Provider, RedirectUri, null)
        {
        }

        public ChallengeResult(string Provider, string RedirectUri, string UserId)
        {
            this.Provider = Provider;
            this.RedirectUri = RedirectUri;
            this.UserId = UserId;
        }

        public string Provider { get; set; }
        public string RedirectUri { get; set; }
        public string UserId { get; set; }

        public override void ExecuteResult(ControllerContext Context)
        {
            var Properties = new Microsoft.Owin.Security.AuthenticationProperties() { RedirectUri = RedirectUri };

            if (UserId != null)
            {
                Properties.Dictionary[XsrfKey] = UserId;
            }

            Context.HttpContext.GetOwinContext().Authentication.Challenge(Properties, Provider);
        }
    }
}
