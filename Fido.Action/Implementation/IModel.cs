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

        Access ReadAccess { get; }
        Access WriteAccess { get; }
    }

    public interface IDataModel
    {
        IList<Dtos.Activity> Denied { get; set; }

        void BuildDenied(Guid UserId);

        bool Allowed(string Action, string Name, string Area);
        bool NotAllowed(string Action, string Name, string Area);
    }

    public interface IModel<TMODEL> : ILogicModel, IDataModel
        where TMODEL : IModel<TMODEL>
    {
        TMODEL Prepare(TMODEL Model);
        TMODEL Read(Guid Id);
        TMODEL Read(IndexOptions IndexOptions);
        bool Save(TMODEL Model);
        bool Confirm(Guid ConfirmationId);
        bool Delete(TMODEL Model);
        void OnInvalidWrite(TMODEL Model);
        void OnFailedWrite(TMODEL Model);
    }
}
