using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoMapper;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

// http://odetocode.com/blogs/scott/archive/2013/03/11/dropdownlistfor-with-asp-net-mvc.aspx

namespace Fido.Action.Models
{
    public class UserVM : Model<UserVM>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public HashSet<string> AllLocalCredentialStates;
        public HashSet<string> AllExternalCredentialStates;
        public IList<RoleVM> AllRoles = new List<RoleVM>();

        public Guid Id { get; set; }

        [Display(Name = "firstname")]
        [Required(ErrorMessage = "The first name field cannot be left blank")]
        public string Firstname { get; set; }

        [Display(Name = "surname")]
        [Required(ErrorMessage = "The surname field cannot be left blank")]
        public string Surname { get; set; }

        [Display(Name = "fullname")]
        public string FirstnameSurname { get; set; }

        [Display(Name = "surname, firstname")]
        public string SurnameFirstname { get; set; }

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

        public IList<Guid> SelectedRoles { get; set; }

        [Display(Name = "created date")]
        public DateTime CreatedUtc { get; set; }
        [Display(Name = "record age")]
        public int? CreatedAgeDays { get; set; }
        public bool IsNew { get; set; }
        public byte[] RowVersion { get; set; }
        #endregion

        public UserVM() { }
        public UserVM(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresReadPermission: true,
                        RequiresWritePermission: true)
        { }

        public override UserVM Prepare(UserVM Model)
        {
            if (Model == null) Model = new UserVM(); // Think on this

            var RoleService = ServiceFactory.CreateService<IRoleService>();
            Model.AllRoles = Mapper.Map<IList<Dtos.Role>, IList<RoleVM>>(RoleService.GetAll().OrderBy(r => r.Name).ToList());

            Model.AllLocalCredentialStates = new HashSet<string>() { "Expired", "Enabled", "Disabled", Model.LocalCredentialState };
            Model.AllExternalCredentialStates = new HashSet<string>() { "Enabled", "Disabled", Model.ExternalCredentialState };

            return Model;
        }

        public override UserVM Read(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();
            
                var User = UserService.Get(Id);
                var Model = Mapper.Map<Dtos.User, UserVM>(User);

                return Model;
            }
        }

        public override bool Save(UserVM Model)
        {
            using (new FunctionLogger(Log))
            {
                var UserDto = Mapper.Map<UserVM, Dtos.User>(Model);
//                UserDto.Roles = new List<Dtos.Role>();

                UserDto.Roles = Model.SelectedRoles == null ? new List<Dtos.Role>()
                : Mapper.Map<IList<RoleVM>, IList<Dtos.Role>>(
                    (from r in Model.AllRoles
                     where (Model.SelectedRoles.Contains(r.Id))
                     select r).ToList());

                var UserService = ServiceFactory.CreateService<IUserService>();

                UserDto = UserService.SaveWithStates(UserDto);
            //    UserService.SetLocalCredentialState(UserDto.Id, Model.LocalCredentialState);
            //    UserService.SetExternalCredentialState(UserDto.Id, Model.ExternalCredentialState);

                FeedbackAPI.DisplaySuccess("The user details have been saved");
                return true;
            }
        }

        public override bool Delete(UserVM Model)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();

                UserService.Delete(Model.Id);

                FeedbackAPI.DisplaySuccess("The user details have been deleted");
                return true;
            }
        }
    }
}
