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
        public IList<User> Users = new List<User>();

        public class User
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

        public override UsersModel Read(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();
                var UserDtos = UserService.GetAll();

                // Jamie: The model will be wrapped in json, so the properties need to match what it is expecting. Here is
                // the reference to use: http://www.codeproject.com/Articles/155422/jQuery-DataTables-and-ASP-NET-MVC-Integration-Part

                foreach (var UserDto in UserDtos)
                    Users.Add(new User
                        { 
                            Id = UserDto.Id, 
                            Firstname = UserDto.Fullname.Firstname, 
                            Surname = UserDto.Fullname.Surname 
                        });

                return this;
            }
        }
    }
}
