using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class ProfileModel : Model<ProfileModel>, IModelCRUD
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public bool IsNew { get; set; }
        public byte[] RowVersion { get; set; }
   //     public string InputState { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; } // Read only

        [Required(ErrorMessage = "The first name field cannot be left blank")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "The surname field cannot be left blank")]
        public string Surname { get; set; }

        [Display(Name = "Display Name")]
        public string DisplayName { get; set; } // Read only

        [Display(Name = "About")]
        public string About { get; set; }

        [Display(Name = "User Image")]
        public byte[] Image { get; set; } // Write only

        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Registered Days")]
        public int RegisteredDays { get; set; } // Read only
        #endregion

        public ProfileModel() { } // pure model
        public ProfileModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresAuthentication: true)
        { }

        public override ProfileModel Read(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();
                var ProfileDto = UserService.GetProfile(Id);

                Id = ProfileDto.Id;
                CreatedUtc = ProfileDto.CreatedUtc;
                IsNew = ProfileDto.IsNew;
                RowVersion = ProfileDto.RowVersion;
                Firstname = ProfileDto.Fullname.Firstname;
                Surname = ProfileDto.Fullname.Surname;
                DisplayName = ProfileDto.Fullname.DisplayName;
                About = ProfileDto.About;
                RegisteredDays = int.Parse(Math.Truncate((DateTime.UtcNow - ProfileDto.CreatedUtc).TotalDays).ToString());

                return this;
            }
        }

        public override bool Write(ProfileModel Model)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();
                var ProfileDto = new Dtos.Profile()
                {
                    Id = Model.Id,
                    CreatedUtc = Model.CreatedUtc,
                    IsNew = Model.IsNew,
                    About = Model.About,
                    RowVersion = Model.RowVersion,
                    Image = Model.Image,
                    Fullname = new Dtos.Fullname
                    {
                        Firstname = Model.Firstname,
                        Surname = Model.Surname,
                        DisplayName = Model.DisplayName
                    }
                };

                UserService.SetProfile(ProfileDto);

                FeedbackAPI.DisplaySuccess("The user profile has been updated");
                return true;
            }
        }
    }
}
