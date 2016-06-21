using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Fido.Action.Models;

namespace Fido.Action.Implementation
{
    public abstract class Model<TMODEL> : IModel<TMODEL>
        where TMODEL : IModel<TMODEL>
    {
        public IList<Dtos.Activity> Permissions { get; set; }

        [ScriptIgnore]
        public IFeedbackAPI FeedbackAPI { protected get; set; }
        [ScriptIgnore]
        public IAuthenticationAPI AuthenticationAPI { protected get; set; }
        [ScriptIgnore]
        public IModelAPI ModelAPI { protected get; set; }

        [ScriptIgnore]
        public bool RequiresReadPermission { get; private set; }
        [ScriptIgnore]
        public bool RequiresWritePermission { get; private set; }

        internal Model(bool RequiresReadPermission, bool RequiresWritePermission)
        {
            this.RequiresReadPermission = RequiresReadPermission;
            this.RequiresWritePermission = RequiresWritePermission;
        }

        public virtual TMODEL Prepare(TMODEL Model) { return Model; }
        public virtual TMODEL Read(Guid Id) { throw new NotImplementedException("Read not implemented"); }
        public virtual TMODEL Read(IndexOptions IndexOptions) { throw new NotImplementedException("Read not implemented"); }
        public virtual bool Save(TMODEL DataModel) { throw new NotImplementedException("Write not implemented");  }
        public virtual bool Delete(TMODEL DataModel) { throw new NotImplementedException("Delete not implemented"); }
        public virtual void OnInvalidWrite(TMODEL DataModel) { }
        public virtual void OnFailedWrite(TMODEL DataModel) { }
    }
}
