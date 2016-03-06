//using AutoMapper;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Fido.Core;
//using Fido.Dtos;
//using Fido.DataAccess;
//using Fido.Service.Exceptions;

//namespace Fido.Service.Implementation
//{
//    internal class UserImageService : IUserImageService
//    {
//        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

//        public UserImage Get(Guid Id)
//        {
//            using (new FunctionLogger(Log))
//            {
//                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
//                {
//                    // TO DO: Update this to get from the UserRepo
//                    var Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
//   //                 var Repository = DataAccessFactory.CreateRepository<IUserImageRepository>(UnitOfWork);
//                    var UserEntity = Repository.Get(Id);

//                    var UserImageDto = Mapper.Map<Entities.User, UserImage>(UserEntity);
//                    return UserImageDto;
//                }
//            }
//        }

//        //public void Upload(ProfileImage ProfileImage)
//        //{
//        //    using (new FunctionLogger(Log))
//        //    {
//        //        using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
//        //        {
//        //            var Repository = DataAccessFactory.CreateRepository<IProfileImageRepository>(UnitOfWork);
//        //            var ProfileImageEntity = Repository.Get(ProfileImage.Id);
//        //            var New = (ProfileImageEntity == null);

//        //            ProfileImageEntity = Mapper.Map<ProfileImage, Entities.ProfileImage>(ProfileImage, ProfileImageEntity);

//        //            if (New)
//        //            {
//        //                Log.Info("Inserting");
//        //                Repository.Insert(ProfileImageEntity);
//        //            }
//        //            else
//        //            {
//        //                Log.Info("Overwriting");
//        //                Repository.Update(ProfileImageEntity);
//        //            }

//        //            UnitOfWork.Commit();
//        //        }
//        //    }
//        //}
//    }
//}
