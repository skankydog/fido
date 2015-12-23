using System;
using System.Web.Script.Serialization;
using Fido.Action.Models;

namespace Fido.Action.Implementation
{
    public abstract class Model<TMODEL> : IModel<TMODEL>
    {
        protected IFeedbackAPI FeedbackAPI;
        protected IAuthenticationAPI AuthenticationAPI;
        protected IModelAPI ModelAPI;

        [ScriptIgnore]
        public bool RequiresAuthentication { get; private set; }

        public Model() { }
        internal Model(
            IFeedbackAPI FeedbackAPI, IAuthenticationAPI AuthenticationAPI, IModelAPI ModelAPI,
            bool RequiresAuthentication)
        {
            this.FeedbackAPI = FeedbackAPI;
            this.AuthenticationAPI = AuthenticationAPI;
            this.ModelAPI = ModelAPI;
            this.RequiresAuthentication = RequiresAuthentication;
        }

        public virtual TMODEL Prepare(TMODEL Model) { return Model; }
        public virtual TMODEL Read(Guid Id) { throw new NotImplementedException("Read not implemented"); }
        public virtual TMODEL Read(IndexOptions IndexOptions) { throw new NotImplementedException("Read not implemented"); }
        public virtual bool Write(TMODEL DataModel) { throw new NotImplementedException("Write not implemented");  }
        public virtual bool Delete(TMODEL DataModel) { throw new NotImplementedException("Delete not implemented"); }
        public virtual void OnInvalidWrite(TMODEL DataModel) { }
        public virtual void OnFailedWrite(TMODEL DataModel) { }
    }
}
