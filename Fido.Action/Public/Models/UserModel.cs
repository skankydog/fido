using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class UserModel : Model<UserModel>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public IList<User> Users = new List<User>();

        public class User
        {
            public Guid Id { get; set; }
            public string Firstname { get; set; }
            public string Surname { get; set; }
        }
        #endregion

        public UserModel() { } // pure model
        public UserModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresAuthentication: true)
        { }

        public override UserModel Read(Guid Id, int Page)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();

                // TO DO

                return this;
            }
        }

        public override bool Write(UserModel Model)
        {
            using (new FunctionLogger(Log))
            {
                // TO DO
                return base.Write(Model); // TO DO
            }
        }
    }
}
