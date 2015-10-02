using System;
using Fido.Action.Models;

namespace Fido.Action.Implementation
{
    internal interface IModel<TMODEL>
    {
        bool RequiresAuthentication { get; }

        TMODEL Read(Guid Id);
        TMODEL Read(Guid Id, IndexParams Params);
        bool Write(TMODEL Model);
        bool Delete(Guid Id);
        void OnInvalidWrite(TMODEL Model);
        void OnFailedWrite(TMODEL Model);
    }
}
