using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoMapper;
using Fido.Core;
using Fido.Service;
using Fido.ViewModel.Implementation;

// http://odetocode.com/blogs/scott/archive/2013/03/11/dropdownlistfor-with-asp-net-mvc.aspx

namespace Fido.ViewModel.Models.Administration
{
    public class User : Model<User>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public HashSet<string> AllLocalCredentialStates;
        public HashSet<string> AllExternalCredentialStates;
        public IList<Role> AllRoles = new List<Role>();

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

        public bool LocalCredentialsArePresent { get; set; }
        public bool LocalCredentialsAreUsable { get; set; }

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

        public bool ExternalCredentialsArePresent { get; set; }
        public bool ExternalCredentialsAreUsable { get; set; }
        [Display(Name = "external credential state")]
        public string ExternalCredentialState { get; set; }

        public bool HasFacebook { get; set; }
        public bool HasTwitter { get; set; }
        public bool HasLinkedIn { get; set; }
        public bool HasGoogle { get; set; }

        public IList<Guid> SelectedRoles { get; set; }

        //[Display(Name = "created date")]
        //public DateTime CreatedUtc { get; set; } // to abstract? you don't have to use it, right?
        //[Display(Name = "record age")]
        //public int? CreatedAgeDays { get; set; } // to abstract? you don't have to use it, right?
        //public bool IsNew { get; set; } // to abstract? you don't have to use it, right?
        //public byte[] RowVersion { get; set; } // to abstract? you don't have to use it, right?
        #endregion

        public User()
            : base(ReadAccess: Access.Permissioned, WriteAccess: Access.Permissioned)
        { }

        public override User Prepare(User Model)
        {
            if (Model == null) Model = new User(); // Think on this

            var RoleService = ServiceFactory.CreateService<IRoleService>();
            Model.AllRoles = Mapper.Map<IList<Dtos.Role>, IList<Role>>(RoleService.GetAll().OrderBy(r => r.Name).ToList());

            Model.AllLocalCredentialStates = new HashSet<string>() { "Expired", "Enabled", "Disabled", Model.LocalCredentialState };
            Model.AllExternalCredentialStates = new HashSet<string>() { "Enabled", "Disabled", Model.ExternalCredentialState };

            return Model;
        }

        public override User Read(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();
            
                var User = UserService.Get(Id);
                var Model = Mapper.Map<Dtos.User, User>(User);

                return Model;
            }
        }

        public override bool Write(User Model)
        {
            using (new FunctionLogger(Log))
            {
                var UserDto = Mapper.Map<User, Dtos.User>(Model);

                UserDto.Roles = Model.SelectedRoles == null ? new List<Dtos.Role>()
                : Mapper.Map<IList<Role>, IList<Dtos.Role>>(
                    (from r in Model.AllRoles
                     where (Model.SelectedRoles.Contains(r.Id))
                     select r).ToList());

                var UserService = ServiceFactory.CreateService<IUserService>();

//                if (Model.IsNew)  // problem!! Need a new model for the create as it only contains 4 fields!! New view!!
//                    UserDto = UserService.CreateAsAdministrator(Model.Firstname, Model.Surname, Model.EmailAddress, "password");
//                else
                    UserDto = UserService.UpdateAsAdministrator(UserDto);

                FeedbackAPI.DisplaySuccess("The user details have been saved");
                return true;
            }
        }

        public override bool Delete(User Model)
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
