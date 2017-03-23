// http://www.codeproject.com/Articles/155422/jQuery-DataTables-and-ASP-NET-MVC-Integration-Part
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoMapper;
using Fido.Core;
using Fido.Service;
using Fido.ViewModel.Implementation;

namespace Fido.ViewModel.Models.Administration
{
    public class ConfirmationIndex : Model<ConfirmationIndex>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public Guid UserId { get; set; }
        public string FirstnameSurname { get; set; }
        #endregion

        public ConfirmationIndex()
            : base(ReadAccess: Access.Permissioned, WriteAccess: Access.NA)
        { }

        public override ConfirmationIndex Read(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();

                var UserDto = UserService.Get(Id);
                var Model = Mapper.Map<Dtos.User, ConfirmationIndex>(UserDto);

                return Model;
            }
        }
    }
}
