using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Fido.ViewModel.Implementation
{
    public abstract class Model<TMODEL> : IModel<TMODEL>
        where TMODEL : IModel<TMODEL>
    {
        #region IDataModel Implementation
        [ScriptIgnore] public Guid Id { get; set; }
        [ScriptIgnore] public DateTime CreatedUtc { get; set; }
        [ScriptIgnore] public int? CreatedAgeDays { get; set; }
        [ScriptIgnore] public bool IsNew { get; set; }
        [ScriptIgnore] public byte[] RowVersion { get; set; }

        [ScriptIgnore] public IList<string> Denied { get; set; }

        [ScriptIgnore] public virtual string ModelName { get { return this.GetType().Name; } }
        [ScriptIgnore] public virtual string ModelArea { get { return this.GetType().Namespace.Split('.').Last(); } }

        //public bool Allowed(string Action, string Name, string Area)
        //{
            //return (from Activity a in DeniedActivities
            //        where a.Name == ModelName &&
            //        a.Area == Namespace &&
            //        a.Action == FunctionName
            //        select a).Count() == 0;
            //return (from string s in Denied
            //        where s == string.Concat(Action, ".", Name, ".", Area)
            //        select s).Count() == 0;
        //    var Full = string.Concat(Action, ".", Name, ".", Area);
        //    return Denied.Contains(Full) == false;
        //}
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

        public Model(Access ReadAccess, Access WriteAccess)
        {
            this.ReadAccess = ReadAccess;
            this.WriteAccess = WriteAccess;
        }

        public virtual TMODEL Prepare(TMODEL Model) { return Model; }
        public virtual TMODEL Read(Guid Id) { throw new NotImplementedException("Read not implemented"); }
        public virtual TMODEL Read(ListOptions IndexOptions) { throw new NotImplementedException("Read not implemented"); }
        public virtual TMODEL Read(Guid Id, ListOptions IndexOptions) { throw new NotImplementedException("Read not implemented"); }
        public virtual bool Confirm(Guid ConfirmationId) { throw new NotImplementedException("Confirm not implemented"); }
        public virtual bool Write(TMODEL DataModel) { throw new NotImplementedException("Write not implemented");  }
        public virtual bool Delete(TMODEL DataModel) { throw new NotImplementedException("Delete not implemented"); }
        public virtual void OnInvalidWrite(TMODEL DataModel) { }
        public virtual void OnFailedWrite(TMODEL DataModel) { }
    }
}
