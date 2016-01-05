using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class ProfileImageModel : Model<ProfileImageModel>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public byte[] Image;
        #endregion

        public ProfileImageModel() { }
        public ProfileImageModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base(FeedbackAPI, LoginAPI, ModelAPI,
                       RequiresReadPermission: false, RequiresWritePermission: false)
        { }

        public override ProfileImageModel Read(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                var UserImageService = ServiceFactory.CreateService<IProfileImageService>();
                var UserImageDto = UserImageService.Download(Id);

                Image = UserImageDto.Image;

                return this;
            }
        }
    }
}
