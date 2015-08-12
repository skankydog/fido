using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class UsersModel : Model<UsersModel>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public IList<VMUser> Users = new List<VMUser>();

        public class VMUser
        {
            public Guid Id { get; set; }
            public string Firstname { get; set; }
            public string Surname { get; set; }
        }
        #endregion

        public UsersModel() { } // pure model
        public UsersModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresAuthentication: true)
        { }

        public override UsersModel Read(Guid Id, int Page)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();
                var UserDtos = UserService.GetPageInSurnameOrder(0, 25); // Magic numbers. Need removal

                // Jamie: The model will be wrapped in json, so the properties need to match what it is expecting. Here is
                // the reference to use: http://www.codeproject.com/Articles/155422/jQuery-DataTables-and-ASP-NET-MVC-Integration-Part

                var UsersModel = new UsersModel();

                foreach (var U in UserDtos)
                    UsersModel.Users.Add(new VMUser { Id = U.Id, Firstname = U.Fullname.Firstname, Surname = U.Fullname.Surname });

                return UsersModel;
            }
        }
    }
}
