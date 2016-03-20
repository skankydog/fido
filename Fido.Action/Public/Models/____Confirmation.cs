//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using Fido.Core;
//using Fido.Service;
//using Fido.Action.Implementation;

//namespace Fido.Action.Models
//{
//    public class Confirmation : Model<Confirmation>
//    {
//        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

//        #region Data
//        public Guid ConfirmationId { get; set; }
//        #endregion

//        public Confirmation() { }
//        public Confirmation(
//            IFeedbackAPI FeedbackAPI,
//            IAuthenticationAPI LoginAPI,
//            IModelAPI ModelAPI)
//                : base (FeedbackAPI, LoginAPI, ModelAPI,
//                        RequiresReadPermission: false, RequiresWritePermission: true)
//        { }

//        public override bool Save(Confirmation Model)
//        {
//            using (new FunctionLogger(Log))
//            {
//                var ConfirmationService = ServiceFactory.CreateService<IConfirmationService>();
//                var ConfirmationType = ConfirmationService.GetConfirmationType(Model.ConfirmationId);

//                //if (ConfirmationType == "Register Local Account")
//                //{
//                //    var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();
//                //    var User = AuthenticationService.RegistrationComplete(Model.ConfirmationId);

//                //    AuthenticationAPI.SignIn(User.Id, User.Fullname.FirstnameSurname, false);

//                //    FeedbackAPI.DisplaySuccess("Thank you for confirming your email address and completing your registration - welcome " + User.Fullname.Firstname + ".");
//                //    return true;
//                //}

//                //if (ConfirmationType == "Change Email Address")
//                //{
//                //    var UserService = ServiceFactory.CreateService<IUserService>();

//                //    UserService.ChangeEmailAddressComplete(Model.ConfirmationId);
//                //    FeedbackAPI.DisplaySuccess("The email address for this account has been successfully changed.");

//                //    return true;
//                //}

//                //if (ConfirmationType == "Register Local Credentials")
//                //{
//                //    var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();
//                //    var User = AuthenticationService.SetLocalCredentialComplete(Model.ConfirmationId);

//                //    AuthenticationAPI.SignOut();
//                //    AuthenticationAPI.SignIn(User.Id, User.Fullname.FirstnameSurname, false);
//                //    AuthenticationAPI.LoggedInCredentialState = User.LocalCredentialState;
//                //    FeedbackAPI.DisplaySuccess("Your local credentials have been confirmed.");

//                //    return true;
//                //}

//                //if (ConfirmationType == "Forgotten Password")
//                //{
//                //    throw new NotImplementedException("Not implemented yet - I believe I need to change backt to one confirmation model per type");
//                //    //return true;
//                //}

//                throw new NotImplementedException("An unexpected error has occurred. The confirmation has not been successful.");
//            }
//        }
//    }
//}
