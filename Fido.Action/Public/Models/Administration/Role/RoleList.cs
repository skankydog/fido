// http://www.codeproject.com/Articles/155422/jQuery-DataTables-and-ASP-NET-MVC-Integration-Part
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Fido.Core;
using Fido.Dtos;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models.Administration
{
    public class RoleList : Model<RoleList>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public string sEcho;
        public int iTotalRecords;
        public int iTotalDisplayRecords;
        public IList<string[]> aaData = new List<string[]>();
        #endregion

        public RoleList()
            : base(ReadAccess: Access.Permissioned, WriteAccess: Access.NA)
        { }

        public override RoleList Read(ListOptions ListOptions)
        {
            using (new FunctionLogger(Log))
            {
                var PageOfRecords = GetPageOfRecords(ListOptions.SortColumn, ListOptions.SortOrder, ListOptions.Skip, ListOptions.Take, ListOptions.Filter);
                var CountUnfiltered = CountAll();
                var CountFiltered = ListOptions.Filter.IsNullOrEmpty() ? CountUnfiltered : PageOfRecords.Count();

                return new RoleList
                {
                    sEcho = ListOptions.Echo,
                    iTotalRecords = CountUnfiltered,
                    iTotalDisplayRecords = CountFiltered,
                    aaData = PageOfRecords
                };
            }
        }

        private IList<string[]> GetPageOfRecords(int SortColumn, char SortOrder, int Skip, int Take, string Filter)
        {
            using (new FunctionLogger(Log))
            {
                var RoleService = ServiceFactory.CreateService<IRoleService>();
                IList<Dtos.Role> RoleDtos;

                switch (SortColumn)
                {
                    case 0:
                        RoleDtos = RoleService.GetPageInNameOrder(SortOrder, Skip, Take, Filter);
                        break;

                    default:
                        RoleDtos = RoleService.GetPageInDefaultOrder(SortOrder, Skip, Take, Filter);
                        break;
                }

                return (from RoleDto in RoleDtos
                        select new[] {
                        RoleDto.Name.Nvl(),
                        RoleDto.Id.ToString(), // Edit
                        RoleDto.Id.ToString()  // Delete
                    }).ToArray();
            }
        }

        private int CountAll()
        {
            using (new FunctionLogger(Log))
            {
                var RoleService = ServiceFactory.CreateService<IRoleService>();
                return RoleService.CountAll();
            }
        }
    }
}
