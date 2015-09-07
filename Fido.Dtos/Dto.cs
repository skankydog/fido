using System;
using System.Collections.Generic;

namespace Fido.Dtos
{
    public abstract class Dto
    {
        public Dto()
        {
            Id = Guid.NewGuid();
            CreatedUtc = DateTime.UtcNow;
            IsNew = true;
        }

        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public int? CreatedAgeDays { get; set; }
        public bool IsNew { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
