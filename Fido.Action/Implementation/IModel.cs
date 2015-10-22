using System;
using Fido.Action.Models;

namespace Fido.Action.Implementation
{
    internal interface IModel
    {
        bool RequiresAuthentication { get; }
    }

    internal interface IModel<TMODEL> : IModel
    {
        TMODEL Read(Guid Id);
        TMODEL Read(Guid Id, IndexOptions IndexOptions);
        bool Write(TMODEL DataModel);
        bool Delete(TMODEL DataModel);
        void OnInvalidWrite(TMODEL DataModel);
        void OnFailedWrite(TMODEL DataModel);
    }
}
