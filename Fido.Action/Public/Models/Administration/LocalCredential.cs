//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using Fido.Core;
//using Fido.Service;
//using Fido.Action.Implementation;

//namespace Fido.Action.Models.Administration
//{
//    public class LocalCredential : Model<LocalCredential>
//    {
//        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

//        #region Data
//        public Guid Id { get; set; }

//        public string EmailAddress { get; set; }
//        public string Password { get; set; }
//        #endregion

//        public LocalCredential()
//            : base(ReadAccess: Access.Permissioned, WriteAccess: Access.Permissioned)
//        { }

//        public override LocalCredential Read(Guid Id)
//        {
//            using (new FunctionLogger(Log))
//            {
//                var UserService = ServiceFactory.CreateService<IUserService>();
//                var UserDto = UserService.Get(Id);

//                var Model = AutoMapper.Mapper.Map<Dtos.User, LocalCredential>(UserDto);

//                return Model;
//            }
//        }

//        public override bool Save(LocalCredential Model)
//        {
//            using (new FunctionLogger(Log))
//            {
//                var UserService = ServiceFactory.CreateService<IUserService>();

//                UserService.CreateLocalCredential(Model.Id, Model.EmailAddress, Model.Password);

//                FeedbackAPI.DisplaySuccess("The local credentials have been created");
//                return true;
//            }
//        }

//        public override bool Delete(LocalCredential Model)
//        {
//            using (new FunctionLogger(Log))
//            {
//                var UserService = ServiceFactory.CreateService<IUserService>();

//                UserService.DeleteLocalCredential(Model.Id);

//                FeedbackAPI.DisplaySuccess("The local credentials have been deleted");
//                return true;
//            }
//        }
//    }
//}
