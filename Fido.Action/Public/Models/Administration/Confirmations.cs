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
    public class Confirmations : Model<Confirmations>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public string sEcho;
        public int iTotalRecords;
        public int iTotalDisplayRecords;
        public IList<string[]> aaData = new List<string[]>();
        #endregion

        public Confirmations()
            : base(ReadAccess: Access.Permissioned, WriteAccess: Access.Permissioned)
        { }

        public override Confirmations Read(Guid Id, IndexOptions IndexOptions)
        {
            using (new FunctionLogger(Log))
            {
                var PageOfRecords = GetPageOfRecords(Id, IndexOptions.SortColumn, IndexOptions.SortOrder, IndexOptions.Skip, IndexOptions.Take, IndexOptions.Filter);
                var CountUnfiltered = CountAll();
                var CountFiltered = IndexOptions.Filter.IsNullOrEmpty() ? CountUnfiltered : PageOfRecords.Count();

                return new Confirmations
                {
                    sEcho = IndexOptions.Echo,
                    iTotalRecords = CountUnfiltered,
                    iTotalDisplayRecords = CountFiltered,
                    aaData = PageOfRecords
                };
            }
        }

        private IList<string[]> GetPageOfRecords(Guid Id, int SortColumn, char SortOrder, int Skip, int Take, string Filter)
        {
            using (new FunctionLogger(Log))
            {
                var ConfirmationService = ServiceFactory.CreateService<IConfirmationService>();
                IList<Dtos.Confirmation> ConfirmationDtos;

                ConfirmationDtos = ConfirmationService.GetPageInDefaultOrder(Id, SortOrder, Skip, Take, Filter);

                return (from ConfirmationDto in ConfirmationDtos
                        select new[] {
                        ConfirmationDto.ConfirmType.Nvl(),
                        ConfirmationDto.EmailAddress.Nvl(),
                        ConfirmationDto.QueuedUTC.ToString(),
                        ConfirmationDto.SentUTC.Nvl(),
                        ConfirmationDto.ReceivedUTC.Nvl(),
                        ConfirmationDto.State.Nvl(),
                        ConfirmationDto.Deletable ? ConfirmationDto.Id.ToString() : string.Empty // Delete
                    }).ToArray();
            }
        }

        private int CountAll()
        {
            using (new FunctionLogger(Log))
            {
                var ConfirmationService = ServiceFactory.CreateService<IConfirmationService>();
                return ConfirmationService.CountAll();
            }
        }
    }
}
