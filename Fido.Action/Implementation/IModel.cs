using System;
using Fido.Action.Models;

namespace Fido.Action.Implementation
{
    internal interface IModel
    {
        string State { get; set; }
    }

    internal interface IModel<TMODEL> : IModel
    {
        bool RequiresAuthentication { get; }
        
        TMODEL Read(Guid Id);
        TMODEL Read(Guid Id, IndexOptions StateOptions);
        bool Write(TMODEL DataModel);
        bool Delete(TMODEL DataModel);
        void OnInvalidWrite(TMODEL DataModel);
        void OnFailedWrite(TMODEL DataModel);
    }
}
