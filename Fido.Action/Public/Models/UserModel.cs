using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

// http://odetocode.com/blogs/scott/archive/2013/03/11/dropdownlistfor-with-asp-net-mvc.aspx

namespace Fido.Action.Models
{
    public class UserModel : Model<UserModel>, IModelCRUD
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public readonly List<string> LocalCredentialStates = new List<string>() {"Expired", "Enabled", "Disabled"};
        public readonly List<string> ExternalCredentialStates = new List<string>() { "Enabled", "Disabled" };

        public Guid Id { get; set; }

        [Required(ErrorMessage = "The first name field cannot be left blank")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "The surname field cannot be left blank")]
        public string Surname { get; set; }

        [Display(Name = "name")]
        public string DisplayName { get; set; }

        public string About { get; set; }

        public bool HasLocalCredentials { get; set; }

        [Display(Name = "email address")]
        public string EmailAddress { get; set; }

        [Display(Name = "local credential state")]
        public string LocalCredentialState { get; set; }

        [Display(Name = "email address change date")]
        public DateTime? EmailAddressLastChangeUtc { get; set; }

        public int? EmailAddressAgeDays { get; set; }

        [Display(Name = "password change date")]
        public DateTime? PasswordLastChangeUtc { get; set; }

        [Display(Name = "password age (days)")]
        public int? PasswordAgeDays { get; set; }

        public bool HasExternalCredentials { get; set; }
        [Display(Name = "external credential state")]
        public string ExternalCredentialState { get; set; }

        public bool HasFacebook = false;
        public bool HasTwitter = false;
        public bool HasLinkedIn = false;
        public bool HasGoogle = false;

        [Display(Name = "created date")]
        public DateTime CreatedUtc { get; set; }
        [Display(Name = "record age")]
        public int? CreatedAgeDays { get; set; }
        public bool IsNew { get; set; }
        public byte[] RowVersion { get; set; }
        #endregion

        public UserModel() { } // pure model
        public UserModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresAuthentication: true)
        { }

        public override UserModel Read(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();
                var UserDto = UserService.Get(Id, "ExternalCredentials");
                var Model = new UserModel
                {
                    Id = UserDto.Id,
                    IsNew = false,
                    CreatedUtc = UserDto.CreatedUtc,
                    RowVersion = UserDto.RowVersion,
                    CreatedAgeDays = UserDto.CreatedAgeDays,
                    EmailAddressAgeDays = UserDto.EmailAddressAgeDays,
                    PasswordAgeDays = UserDto.PasswordAgeDays,
                    EmailAddress = UserDto.EmailAddress,
                    EmailAddressLastChangeUtc = UserDto.EmailAddressLastChangeUtc,
                    Firstname = UserDto.Fullname.Firstname,
                    Surname = UserDto.Fullname.Surname,
                    About = UserDto.About,
                    HasLocalCredentials = UserDto.HasLocalCredentials,
                    LocalCredentialState = UserDto.LocalCredentialState,
                    PasswordLastChangeUtc = UserDto.PasswordLastChangeUtc,
                    HasExternalCredentials = UserDto.HasExternalCredentials,
                    ExternalCredentialState = UserDto.ExternalCredentialState,
                    HasFacebook = UserDto.HasLoginProvider("facebook"),
                    HasTwitter = UserDto.HasLoginProvider("twitter"),
                    HasLinkedIn = UserDto.HasLoginProvider("linkedin"),
                    HasGoogle = UserDto.HasLoginProvider("google")
                };

                // Just in case the current state is not one of the ones we want to allow
                // the user to set, we use the below extention methods to ensure that if
                // this is the case, the current state is added to the list...
                Model.LocalCredentialStates.SmartAdd(UserDto.LocalCredentialState);
                Model.ExternalCredentialStates.SmartAdd(UserDto.ExternalCredentialState);

                return Model;
            }
        }

        public override bool Write(UserModel Model)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();
                //var UserDto = new Dtos.User
                    //{
                    //    Id = Model.Id,
                    //    IsNew = Model.IsNew,
                    //    CreatedUtc = Model.CreatedUtc,
                    //    RowVersion = Model.RowVersion,
                    //    EmailAddress = Model.EmailAddress,
                    //    EmailAddressLastChangeUtc = Model.EmailAddressLastChangeUtc,
                    //    Fullname = new Dtos.Fullname
                    //        {
                    //            Firstname = Model.Firstname,
                    //            Surname = Model.Surname
                    //        },
                    //    About = Model.About,

                    //    // Only if is not new????????
                    //    LocalCredentialState = Model.LocalCredentialState == null ? "None" : Model.LocalCredentialState,
                    //    ExternalCredentialState = Model.ExternalCredentialState == null ? "None" : Model.ExternalCredentialState
                    //};

                //UserService.Update(UserDto);
                var UserDto = Mapper.Map<UserModel, Dtos.User>(Model);
                UserService.Save(UserDto);
                //
                // Additionally, update the email address and credential states from the view model via
                // service calls...
           //     UserService.SetEmailAddress(Model.EmailAddress);
           //     UserService.SetPassword(Model.Password);
           //     UserService.SetLocalCredentialState(Model.LocalCredentialState);
           //     UserService.SetExternalCredentialState(Model.ExternalCredentialState);

                FeedbackAPI.DisplaySuccess("The user details have been updated");

                return true;
            }
        }
    }
}
