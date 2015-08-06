using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities
{
    public class ProfileImage : Entity
    {
        public byte[] Image { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
