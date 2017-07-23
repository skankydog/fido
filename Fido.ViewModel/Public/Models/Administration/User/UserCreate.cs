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
    public class UserCreate : Model<UserCreate>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        [Display(Name = "firstname")]
        [Required(ErrorMessage = "The first name field cannot be left blank")]
        public string Firstname { get; set; }

        [Display(Name = "surname")]
        [Required(ErrorMessage = "The surname field cannot be left blank")]
        public string Surname { get; set; }

        [Display(Name = "email address")]
        [Required(ErrorMessage = "The email address field cannot be left blank")]
        public string EmailAddress { get; set; }

        [Display(Name = "password")]
        [Required(ErrorMessage = "The password field cannot be left blank")]
        public string Password { get; set; }
        #endregion

        public UserCreate()
            : base(ReadAccess: Access.NA, WriteAccess: Access.Permissioned)
        { }

        public override bool Write(UserCreate Model)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();
                var UserDto = UserService.CreateAsAdministrator(Model.Id, Model.Firstname, Model.Surname, Model.EmailAddress, Model.Password);

                FeedbackAPI.DisplaySuccess("The user details have been saved");
                return true;
            }
        }
    }
}
