// http://www.codeproject.com/Articles/155422/jQuery-DataTables-and-ASP-NET-MVC-Integration-Part
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class UsersModel : Model<UsersModel>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public string sEcho;
        public int iTotalRecords;
        public int iTotalDisplayRecords;
        public IList<string[]> aaData = new List<string[]>();
        #endregion

        public UsersModel() { } // pure model
        public UsersModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresAuthentication: true)
        { }

        public override UsersModel Read(Guid Id, IndexOptions Params)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();
                var Delegates = new Dictionary<int, Func<int, int, char, string, IList<Dtos.User>>>()
                    { { 0, (s, t, o, f) => UserService.GetPageInFirstnameOrder(s, t, o, f) },
                      { 1, (s, t, o, f) => UserService.GetPageInSurnameOrder(s, t, o, f) },
                      { 2, (s, t, o, f) => UserService.GetPageInEmailAddressOrder(s, t, o, f) },
                      { 3, (s, t, o, f) => UserService.GetPageInLocalCredentialOrder(s, t, o, f) },
                      { 4, (s, t, o, f) => UserService.GetPageInExternalCredentialOrder(s, t, o, f) },
                      { 5, (s, t, o, f) => UserService.GetPageInDefaultOrder(s, t, o, f) },   // Edit - not sortable
                      { 6, (s, t, o, f) => UserService.GetPageInDefaultOrder(s, t, o, f) } }; // Delete - not sortable

                var Users = Delegates[Params.SortColumn](Params.Skip, Params.Take, Params.SortOrder, Params.Filter);
                var CountAll = UserService.CountAll();
                var CountDisplay = Params.Filter.IsNullOrEmpty() ? CountAll : Users.Count();

                return new UsersModel
                {
                    sEcho = Params.Echo,
                    iTotalRecords = CountAll,
                    iTotalDisplayRecords = CountDisplay,
                    aaData = (
                        from Dto in Users
                        select new[] {
                            Dto.Fullname.Firstname,
                            Dto.Fullname.Surname,
                            Dto.EmailAddress.Nvl(),
                            Dto.LocalCredentialState,
                            Dto.ExternalCredentialState,
                            Dto.Id.ToString(), // Edit
                            Dto.Id.ToString()  // Delete
                          }).ToArray()
                };
            }
        }
    }
}
