using System;
using System.Collections.Generic;
using Fido.Action.Models;

namespace Fido.Action.Implementation
{
    public interface ILogicModel
    {
        IFeedbackAPI FeedbackAPI { set; }
        IAuthenticationAPI AuthenticationAPI { set; }
        IModelAPI ModelAPI { set; }

        bool RequiresReadPermission { get; }
        bool RequiresWritePermission { get; }
    }

   public interface IDataModel
    {
       IList<Dtos.Activity> Permissions { get; set; }
    }

    public interface IModel<TMODEL> : ILogicModel, IDataModel
        where TMODEL : IModel<TMODEL>
    {
        TMODEL Prepare(TMODEL Model);
        TMODEL Read(Guid Id);
        TMODEL Read(IndexOptions IndexOptions);
        bool Save(TMODEL Model);
        bool Delete(TMODEL Model);
        void OnInvalidWrite(TMODEL Model);
        void OnFailedWrite(TMODEL Model);
    }
}
