using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fido.Core;
using Fido.Dtos;
using Fido.DataAccess;
using Fido.Service.Exceptions;

namespace Fido.Service.Implementation
{
    internal class ProfileImageService : IProfileImageService
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ProfileImage Download(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var Repository = DataAccessFactory.CreateRepository<IProfileImageRepository>(UnitOfWork);
                    var ProfileImageEntity = Repository.Get(Id);

                    ProfileImage ProfileImageDto = Mapper.Map<Entities.ProfileImage, ProfileImage>(ProfileImageEntity);
                    return ProfileImageDto;
                }
            }
        }

        //public void Upload(ProfileImage ProfileImage)
        //{
        //    using (new FunctionLogger(Log))
        //    {
        //        using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
        //        {
        //            var Repository = DataAccessFactory.CreateRepository<IProfileImageRepository>(UnitOfWork);
        //            var ProfileImageEntity = Repository.Get(ProfileImage.Id);
        //            var New = (ProfileImageEntity == null);

        //            ProfileImageEntity = Mapper.Map<ProfileImage, Entities.ProfileImage>(ProfileImage, ProfileImageEntity);

        //            if (New)
        //            {
        //                Log.Info("Inserting");
        //                Repository.Insert(ProfileImageEntity);
        //            }
        //            else
        //            {
        //                Log.Info("Overwriting");
        //                Repository.Update(ProfileImageEntity);
        //            }

        //            UnitOfWork.Commit();
        //        }
        //    }
        //}
    }
}
