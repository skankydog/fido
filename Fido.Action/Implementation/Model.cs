using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Fido.Action.Models;

namespace Fido.Action.Implementation
{
    public abstract class Model<TMODEL> : IModel<TMODEL>
        where TMODEL : IModel<TMODEL>
    {
        #region IDataModel Implementation
        [ScriptIgnore]
        public IList<Dtos.Activity> Permissions { get; set; }

        //public bool HasWritePermission(string Name, string Area)
        //{
        //    return (from p in Permissions
        //            where p.Name == Name &&
        //                  p.Area == Area &&
        //                  p.Action == Permission.Write.ToString()
        //            select p).Count() == 1;
        //}

        //public bool HasReadPermission(string Name, string Area)
        //{
        //    return (from p in Permissions
        //            where p.Name == Name &&
        //                  p.Area == Area &&
        //                  p.Action == Permission.Read.ToString()
        //            select p).Count() == 1;
        //}

        //public bool HasReadOrWritePermission(string Name, string Area)
        //{
        //    return (from p in Permissions
        //            where p.Name == Name &&
        //                  p.Area == Area
        //            select p).Count() > 0;
        //}

        //public bool HasWritePermissions(string Area)
        //{
        //    return (from p in Permissions
        //            where p.Area == Area &&
        //                  p.Action == Permission.Write.ToString()
        //            select p).Count() > 0;
        //}

        //public bool HasReadPermissions(string Area)
        //{
        //    return (from p in Permissions
        //            where p.Area == Area &&
        //                  p.Action == Permission.Read.ToString()
        //            select p).Count() > 0;
        //}

        public bool HasArea(string Area)
        {
            return (from p in Permissions
                    where p.Area == Area
                    select p).Count() > 0;
        }
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
