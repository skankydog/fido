using System;

namespace Fido.Action.Implementation
{
    public interface IModelCRUD
    {
        Guid Id { get; set; }
        DateTime CreatedUtc { get; set; }
        bool IsNew { get; set; }
        byte[] RowVersion { get; set; }
    }
}
