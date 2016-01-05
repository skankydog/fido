using System;
using Fido.Action.Models;

namespace Fido.Action.Implementation
{
    internal interface IModel
    {
        bool RequiresReadPermission { get; }
        bool RequiresWritePermission { get; }
    }

    internal interface IModel<TMODEL> : IModel
    {
        TMODEL Prepare(TMODEL Model);
        TMODEL Read(Guid Id);
        TMODEL Read(IndexOptions IndexOptions);
        bool Write(TMODEL DataModel);
        bool Delete(TMODEL DataModel);
        void OnInvalidWrite(TMODEL DataModel);
        void OnFailedWrite(TMODEL DataModel);
    }
}
