using System;
using System.Web.Script.Serialization;

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

        public virtual TMODEL Read(Guid Id) { throw new NotImplementedException("Read not implemented"); }
        public virtual TMODEL Read(Guid Id, int Page) { throw new NotImplementedException("Read not implemented"); }
        public virtual bool Write(TMODEL Model) { throw new NotImplementedException("Write not implemented");  }
        public virtual bool Delete(Guid Id) { throw new NotImplementedException("Delete not implemented"); }
        public virtual void OnInvalidWrite(TMODEL Model) { }
        public virtual void OnFailedWrite(TMODEL Model) { }
    }
}
