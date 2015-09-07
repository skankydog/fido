using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public string About { get; set; }

        public bool HasLocalCredentials { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Local Credential State")]
        public string LocalCredentialState { get; set; }

        [Display(Name = "Email Address Change Date")]
        public DateTime? EmailAddressLastChangeUtc { get; set; }

        public int? EmailAddressAgeDays { get; set; }

        [Display(Name = "Password Change Date")]
        public DateTime? PasswordLastChangeUtc { get; set; }

        [Display(Name = "Password Age (Days)")]
        public int? PasswordAgeDays { get; set; }

        public bool HasExternalCredentials { get; set; }
        [Display(Name = "External Credential State")]
        public string ExternalCredentialState { get; set; }

        public bool HasFacebook = false;
        public bool HasTwitter = false;
        public bool HasLinkedIn = false;
        public bool HasGoogle = false;

   //     public IList<ExternalCredential> ExternalCredentials { get; set; } // Not sure if we want this or not yet

        [Display(Name = "Created Date")]
        public DateTime CreatedUtc { get; set; }
        [Display(Name = "Record Age")]
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

                // Cater for allowing the state of the user to be changed from non-selectable
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
                var UserDto = new Dtos.User
                    {
                        Id = Model.Id,
                        IsNew = Model.IsNew,
                        CreatedUtc = Model.CreatedUtc,
                        RowVersion = Model.RowVersion,
                        EmailAddress = Model.EmailAddress,
                        EmailAddressLastChangeUtc = Model.EmailAddressLastChangeUtc,
                        Fullname = new Dtos.Fullname
                            {
                                Firstname = Model.Firstname,
                                Surname = Model.Surname
                            },
                        About = Model.About,
                        LocalCredentialState = Model.LocalCredentialState,
                        ExternalCredentialState = Model.ExternalCredentialState
                    };

                UserService.Update(UserDto);
                FeedbackAPI.DisplaySuccess("The user details have been updated");

                return true;
            }
        }
    }
}
