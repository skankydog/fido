using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class ProfileVM : Model<ProfileVM>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public Guid Id { get; set; } // User entity
        public DateTime CreatedUtc { get; set; } // User entity
        public bool IsNew { get; set; } // User entity
        public byte[] RowVersion { get; set; } // User entity

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; } // Read only

        [Required(ErrorMessage = "The first name field cannot be left blank")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "The surname field cannot be left blank")]
        public string Surname { get; set; }

        [Display(Name = "Display Name")]
        public string FirstnameSurname { get; set; } // Read only

        [Display(Name = "About")]
        public string About { get; set; }

        [Display(Name = "User Image")]
        public byte[] Image { get; set; } // Write only

        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Account Age (Days)")]
        public int CreatedAgeDays { get; set; } // Read only
        #endregion

        public ProfileVM()
        {
            Id = Guid.NewGuid();
            CreatedUtc = DateTime.UtcNow;
            IsNew = true;
        }

        public ProfileVM(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresReadPermission: true, RequiresWritePermission: true)
        { }

        public override ProfileVM Read(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();

                var ProfileDto = UserService.GetProfile(Id);
                var Model = AutoMapper.Mapper.Map<Dtos.Profile, ProfileVM>(ProfileDto);

                return Model;
            }
        }

        public override bool Save(ProfileVM Model)
        {
            using (new FunctionLogger(Log))
            {
                var ProfileDto = AutoMapper.Mapper.Map<ProfileVM, Dtos.Profile>(Model);

                var UserService = ServiceFactory.CreateService<IUserService>();
                UserService.SetProfile(ProfileDto);

                FeedbackAPI.DisplaySuccess("Your user profile has been updated");
                return true;
            }
        }
    }
}
