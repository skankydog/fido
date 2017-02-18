using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoMapper;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

// http://odetocode.com/blogs/scott/archive/2013/03/11/dropdownlistfor-with-asp-net-mvc.aspx

namespace Fido.Action.Models.Administration
{
    public class Confirmation : Model<Confirmation>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public Guid UserId { get; set; }
        public string ConfirmType { get; set; }
        public string EmailAddress { get; set; }
        public DateTime QueuedUTC { get; set; }
        public DateTime? SentUTC { get; set; }
        public DateTime? ReceivedUTC { get; set; }
        public string State { get; set; }
        public bool Deletable { get; set; }
        #endregion

        public Confirmation()
            : base(ReadAccess: Access.Permissioned, WriteAccess: Access.Permissioned)
        { }

        public override Confirmation Read(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                var ConfirmationService = ServiceFactory.CreateService<IConfirmationService>();

                var Confirmation = ConfirmationService.Get(Id);
                var Model = Mapper.Map<Dtos.Confirmation, Confirmation>(Confirmation);

                return Model;
            }
        }

        public override bool Delete(Confirmation Model)
        {
            using (new FunctionLogger(Log))
            {
                var ConfirmationService = ServiceFactory.CreateService<IConfirmationService>();

                ConfirmationService.Delete(Model.Id);

                FeedbackAPI.DisplaySuccess("The confirmation has been deleted");
                return true;
            }
        }
    }
}
