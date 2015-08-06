using System;

namespace Fido.Action.Implementation
{
    public abstract class Model<TMODEL> : IHandler<TMODEL>, IModel
        where TMODEL : IModel
    {
        protected IFeedbackAPI FeedbackAPI;
        protected IAuthenticationAPI AuthenticationAPI;
        protected IModelAPI ModelAPI;

        public bool RequiresAuthentication { get; private set; }

        #region Data
        public Guid Id { get; set; } // Hidden
        public DateTime CreatedUtc { get; set; } // Hidden
        public bool IsNew { get; set; } // Hidden
        public byte[] RowVersion { get; set; } // Hidden
        public string FormState { get; set; } // Hidden
        #endregion

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
        public virtual void OnInvalidWrite(TMODEL Model) { }
        public virtual void OnFailedWrite(TMODEL Model) { }
    }
}
