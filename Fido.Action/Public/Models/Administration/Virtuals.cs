// http://www.codeproject.com/Articles/155422/jQuery-DataTables-and-ASP-NET-MVC-Integration-Part
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Fido.Core;
using Fido.Dtos;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class Virtuals : Model<Virtuals>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public string sEcho;
        public int iTotalRecords;
        public int iTotalDisplayRecords;
        public IList<string[]> aaData = new List<string[]>();
        #endregion

        public Virtuals() { }
        public Virtuals(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresReadPermission: true, RequiresWritePermission: true)
        { }

        public override Virtuals Read(IndexOptions IndexOptions)
        {
            using (new FunctionLogger(Log))
            {
                var PageOfRecords = GetPageOfRecords(IndexOptions.SortColumn, IndexOptions.SortOrder, IndexOptions.Skip, IndexOptions.Take, IndexOptions.Filter);
                var CountUnfiltered = CountAll();
                var CountFiltered = IndexOptions.Filter.IsNullOrEmpty() ? CountUnfiltered : PageOfRecords.Count();

                return new Virtuals
                {
                    sEcho = IndexOptions.Echo,
                    iTotalRecords = CountUnfiltered,
                    iTotalDisplayRecords = CountFiltered,
                    aaData = PageOfRecords
                };
            }
        }

        private IList<string[]> GetPageOfRecords(int SortColumn, char SortOrder, int Skip, int Take, string Filter)
        {
            return new List<string[]>();
            //using (new FunctionLogger(Log))
            //{
            //    var UserService = ServiceFactory.CreateService<IUserService>();
            //    IList<User> UserDtos;

            //    switch (SortColumn)
            //    {
            //        case 0:
            //            UserDtos = UserService.GetPageInFirstnameOrder(SortOrder, Skip, Take, Filter);
            //            break;

            //        case 1:
            //            UserDtos = UserService.GetPageInSurnameOrder(SortOrder, Skip, Take, Filter);
            //            break;

            //        case 2:
            //            UserDtos = UserService.GetPageInEmailAddressOrder(SortOrder, Skip, Take, Filter);
            //            break;

            //        case 3:
            //            UserDtos = UserService.GetPageInLocalCredentialOrder(SortOrder, Skip, Take, Filter);
            //            break;

            //        case 4:
            //            UserDtos = UserService.GetPageInExternalCredentialOrder(SortOrder, Skip, Take, Filter);
            //            break;

            //        default:
            //            UserDtos = UserService.GetPageInDefaultOrder(SortOrder, Skip, Take, Filter);
            //            break;
            //    }

            //    return (from UserDto in UserDtos
            //            select new[] {
            //            UserDto.Fullname.Firstname.Nvl(),
            //            UserDto.Fullname.Surname.Nvl(),
            //            UserDto.EmailAddress.Nvl(),
            //            UserDto.LocalCredentialState.Nvl(),
            //            UserDto.ExternalCredentialState.Nvl(),
            //            UserDto.Id.ToString(), // Edit
            //            UserDto.Id.ToString()  // Delete
            //        }).ToArray();
            //}
        }

        private int CountAll()
        {
            return 0;
            //using (new FunctionLogger(Log))
            //{
            //    var UserService = ServiceFactory.CreateService<IUserService>();
            //    return UserService.CountAll();
            //}
        }
    }
}
