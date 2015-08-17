using System;

namespace Fido.Action.Implementation
{
    internal interface IModel<TMODEL>
    {
        bool RequiresAuthentication { get; }

        TMODEL Read(Guid Id);
        TMODEL Read(Guid Id, int Page);
        bool Write(TMODEL Model);
        bool Delete(Guid Id);
        void OnInvalidWrite(TMODEL Model);
        void OnFailedWrite(TMODEL Model);
    }
}
