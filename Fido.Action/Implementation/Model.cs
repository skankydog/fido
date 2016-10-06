using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Fido.Action.Models;
using Fido.Service;

namespace Fido.Action.Implementation
{
    public abstract class Model<TMODEL> : IModel<TMODEL>
        where TMODEL : IModel<TMODEL>
    {
        #region IDataModel Implementation
        [ScriptIgnore]
        public IList<Dtos.Activity> DeniedActivities { get; set; }

        public void BuildDeniedActivities(Guid UserId)
        {
            DeniedActivities = new List<Dtos.Activity>();

            if (AuthenticationAPI.Authenticated)
            {
                var UserService = ServiceFactory.CreateService<IUserService>();
                var Activities = UserService.GetDeniedActivities(AuthenticationAPI.AuthenticatedId);

                DeniedActivities = (from Dtos.Activity a in Activities
                          select a).ToList();
            }
        }

        public bool Allowed(string FunctionName, string ModelName, string Namespace)
        {
            return (from Dtos.Activity a in DeniedActivities
                    where a.Name == ModelName &&
                    a.Area == Namespace &&
                    a.Action == FunctionName
                    select a).Count() == 0;
        }
        #endregion

        #region ILogicModel Implementation
        [ScriptIgnore]
        public IFeedbackAPI FeedbackAPI { get; set; }
        [ScriptIgnore]
        public IAuthenticationAPI AuthenticationAPI { get; set; }
        [ScriptIgnore]
        public IModelAPI ModelAPI { get; set; }

        [ScriptIgnore]
        public Access ReadAccess { get; private set; }
        [ScriptIgnore]
        public Access WriteAccess { get; private set; }
        #endregion

        internal Model(Access ReadAccess, Access WriteAccess)
        {
            this.ReadAccess = ReadAccess;
            this.WriteAccess = WriteAccess;
        }

        public virtual TMODEL Prepare(TMODEL Model) { return Model; }
        public virtual TMODEL Read(Guid Id) { throw new NotImplementedException("Read not implemented"); }
        public virtual TMODEL Read(IndexOptions IndexOptions) { throw new NotImplementedException("Read not implemented"); }
        public virtual bool Confirm(Guid ConfirmationId) { throw new NotImplementedException("Confirm not implemented"); }
        public virtual bool Save(TMODEL DataModel) { throw new NotImplementedException("Write not implemented");  }
        public virtual bool Delete(TMODEL DataModel) { throw new NotImplementedException("Delete not implemented"); }
        public virtual void OnInvalidWrite(TMODEL DataModel) { }
        public virtual void OnFailedWrite(TMODEL DataModel) { }
    }
}
