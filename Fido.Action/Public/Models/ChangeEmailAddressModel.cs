﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class ChangeEmailAddressModel : Model<ChangeEmailAddressModel>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        [Required]
        [EmailAddress]
        [Display(Name = "email address")]
        public string EmailAddress { get; set; }
        #endregion

        public ChangeEmailAddressModel() { }
        public ChangeEmailAddressModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresReadPermission: true, RequiresWritePermission: true)
        { }

        public override bool Write(ChangeEmailAddressModel Model)
        {
            using (new FunctionLogger(Log))
            {
                var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

                if (!AuthenticationService.EmailAddressIsFree(Model.EmailAddress))
                {
                    ModelAPI.PropertyError("EmailAddress", "The email address is already in use");
                    return false;
                }

                if (!AuthenticationService.EmailAddressPassesValidation(Model.EmailAddress))
                {
                    ModelAPI.PropertyError("EmailAddress", "The email address is not of a valid format");
                    return false;
                }

                var UserService = ServiceFactory.CreateService<IUserService>();
                var ConfirmationId = UserService.InitiateChangeEmailAddress(AuthenticationAPI.AuthenticatedId, Model.EmailAddress);

                //if (System.Configuration.ConfigurationManager.AppSettings["UI-Mode"] == "Development" ||
                //    System.Configuration.ConfigurationManager.AppSettings["UI-Mode"] == "Test")
                //{
                //    var ConfirmationLink = Url.Action("CompleteForgottenLocalPassword") + "?" + ConfirmationId.ToString();
                //    UserInterface.DisplayInfo("For development use only, click the below link to simulate response to the email.",
                //        new FlashLinks { Links = new List<string> { ConfirmationLink } });
                //}
                //else
                FeedbackAPI.DisplaySuccess("An email will shortly be sent to your nominated email address for confirmation - once confirmed, your details will be updated");

                return true;
            }
        }
    }
}
