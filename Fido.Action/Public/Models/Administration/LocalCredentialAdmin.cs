﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class LocalCredentialAdmin : Model<LocalCredentialAdmin>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public Guid Id { get; set; }

        public string EmailAddress { get; set; }
        public string Password { get; set; }
        #endregion

        public LocalCredentialAdmin() { }
        public LocalCredentialAdmin(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresReadPermission: true, RequiresWritePermission: true)
        { }

        public override LocalCredentialAdmin Read(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();
                var UserDto = UserService.Get(Id);

                var Model = AutoMapper.Mapper.Map<Dtos.User, LocalCredentialAdmin>(UserDto);

                return Model;
            }
        }

        public override bool Save(LocalCredentialAdmin Model)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();

                UserService.CreateLocalCredential(Model.Id, Model.EmailAddress, Model.Password);

                FeedbackAPI.DisplaySuccess("The local credentials have been created");
                return true;
            }
        }

        public override bool Delete(LocalCredentialAdmin Model)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();

                UserService.DeleteLocalCredential(Model.Id);

                FeedbackAPI.DisplaySuccess("The local credentials have been deleted");
                return true;
            }
        }
    }
}