using System;

namespace Fido.Action.Implementation
{
    public interface IModel
    {
        Guid Id { get; set; } // Hidden
        DateTime CreatedUtc { get; set; } // Hidden
        bool IsNew { get; set; } // Hidden
        byte[] RowVersion { get; set; } // Hidden
        string FormState { get; set; } // Hidden
    }
}
