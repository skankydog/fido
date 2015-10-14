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
        public HashSet<string> LocalCredentialStates;
        public HashSet<string> ExternalCredentialStates;

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

        public bool HasFacebook;
        public bool HasTwitter;
        public bool HasLinkedIn;
        public bool HasGoogle;

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

                var Model = Mapper.Map<Dtos.User, UserModel>(UserDto);

                return Model;
            }
        }

        public override bool Write(UserModel Model)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();
                var UserDto = Mapper.Map<UserModel, Dtos.User>(Model);

                UserService.Save(UserDto);
                UserService.SetLocalCredentialState(UserDto.Id, Model.LocalCredentialState);
                UserService.SetExternalCredentialState(UserDto.Id, Model.ExternalCredentialState);

                FeedbackAPI.DisplaySuccess("The user details have been updated");
                return true;
            }
        }

        public override bool Delete(UserModel Model)
        {
            // TO DO: Call UserModel.Delete(Model);

            FeedbackAPI.DisplaySuccess("The user record has been deleted");
            return true;
        }
    }
}
