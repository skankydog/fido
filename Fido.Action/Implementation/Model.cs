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
        public IList<Dtos.Activity> Denied { get; set; }

        public void BuildDenied(Guid UserId)
        {
            Denied = new List<Dtos.Activity>();

            Guid Id = AuthenticationAPI.AuthenticatedId;

            if (Id != Guid.Empty)
            {
                var UserService = ServiceFactory.CreateService<IUserService>();
                var Activities = UserService.GetDeniedActivities(Id);

                Denied = (from Dtos.Activity a in Activities
                          select a).ToList();
            }
        }

        public bool Allowed(string Action, string Name, string Area)
        {
            return (from Dtos.Activity a in Denied
                    where a.Name == Name && 
                    a.Area == Area &&
                    a.Action == Action
                    select a).Count() == 0;
        }

        public bool NotAllowed(string Action, string Name, string Area)
        {
            string ModelAction = "";

            switch (Action)
            {
                case "Update|Get":
                    ModelAction = "Read";
                    break;

                case "Update|Post":
                    ModelAction = "Write";
                    break;

                case "Create|Get":
                    ModelAction = "Write";
                    break;

                case "Create|Post":
                    ModelAction = "Write";
                    break;

                    // ...
            }

            // now build the activity record and see if in the list - if in the list, check to see if we have it
            // If not there at all, then return true (I think)
            return true; // for now
        }

        //public bool HasArea(string Area)
        //{
        //    return (from p in Permissions
        //            where p.Area == Area
        //            select p).Count() > 0;
        //}
        #endregion

        [ScriptIgnore]
        public IFeedbackAPI FeedbackAPI { protected get; set; }
        [ScriptIgnore]
        public IAuthenticationAPI AuthenticationAPI { protected get; set; }
        [ScriptIgnore]
        public IModelAPI ModelAPI { protected get; set; }

        //[ScriptIgnore]
        //public bool ReadPermissioned { get; private set; }
        //[ScriptIgnore]
        //public bool WritePermissioned { get; private set; }

        [ScriptIgnore]
        public Access ReadAccess { get; private set; }
        [ScriptIgnore]
        public Access WriteAccess { get; private set; }

        internal Model(Access ReadAccess, Access WriteAccess)
        {
            this.ReadAccess = ReadAccess;
            this.WriteAccess = WriteAccess;
        }

        //internal Model(bool RequiresReadPermission, bool RequiresWritePermission)
        //{
        //    this.ReadPermissioned = RequiresReadPermission;
        //    this.WritePermissioned = RequiresWritePermission;
        //}

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
