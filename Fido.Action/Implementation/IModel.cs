using System;
using System.Collections.Generic;
using Fido.Action.Models;

namespace Fido.Action.Implementation
{
    public interface ILogicModel
    {
        IFeedbackAPI FeedbackAPI { get;  set; }
        IAuthenticationAPI AuthenticationAPI { get; set; }
        IModelAPI ModelAPI { get; set; }

        Access ReadAccess { get; }
        Access WriteAccess { get; }
    }

    public interface IDataModel
    {
        Guid Id { get; set; }

        IList<Dtos.Activity> DeniedActivities { get; set; }

        void BuildDeniedActivities(Guid UserId);
        bool Allowed(string Action, string Name, string Area);
    }

    public interface IModel<TMODEL> : ILogicModel, IDataModel
        where TMODEL : IModel<TMODEL>
    {
        TMODEL Prepare(TMODEL Model);
        TMODEL Read(Guid Id);
        TMODEL Read(IndexOptions IndexOptions);
        TMODEL Read(Guid Id, IndexOptions IndexOptions);
        bool Save(TMODEL Model);
        bool Confirm(Guid ConfirmationId);
        bool Delete(TMODEL Model);
        void OnInvalidWrite(TMODEL Model);
        void OnFailedWrite(TMODEL Model);
    }
}
