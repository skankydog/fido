using System;
using System.Collections.Generic;
//using Fido.Action.Models.Administration;

namespace Fido.Action.Implementation
{
    public interface ILogicModel
    {
        IFeedbackAPI FeedbackAPI { get;  set; }
        IAuthenticationAPI AuthenticationAPI { get; set; }
        IModelAPI ModelAPI { get; set; }

        Access ReadAccess { get; }
        Access WriteAccess { get; }

        string ModelName { get; }
        string ModelArea { get; }
    }

    public interface IDataModel
    {
        Guid Id { get; set; }
        DateTime CreatedUtc { get; set; }
        int? CreatedAgeDays { get; set; }
        bool IsNew { get; set; }
        byte[] RowVersion { get; set; }

        IList<string> Denied { get; set; }

//        bool Allowed(string Action, string Name, string Area);


    }

    public interface IModel<TMODEL> : ILogicModel, IDataModel
        where TMODEL : IModel<TMODEL>
    {
        TMODEL Prepare(TMODEL Model);
        TMODEL Read(Guid Id);
        TMODEL Read(ListOptions IndexOptions);
        TMODEL Read(Guid Id, ListOptions IndexOptions);
        bool Save(TMODEL Model);
        bool Confirm(Guid ConfirmationId);
        bool Delete(TMODEL Model);
        void OnInvalidWrite(TMODEL Model);
        void OnFailedWrite(TMODEL Model);
    }
}
