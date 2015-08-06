using System;

namespace Fido.Action.Implementation
{
    internal interface IHandler<TMODEL>
    {
        bool RequiresAuthentication { get; }

        TMODEL Read(Guid Id);
        TMODEL Read(Guid Id, int Page);
        bool Write(TMODEL Model);
        void OnInvalidWrite(TMODEL Model);
        void OnFailedWrite(TMODEL Model);
    }
}
