using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities
{
    public abstract class Entity
    {
        public Entity()
        {
            Id = Guid.NewGuid();
            CreatedUtc = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public byte[] RowVersion { get; set; } // ...for optimistic concurrency
    }
}
