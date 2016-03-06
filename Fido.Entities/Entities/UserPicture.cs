using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities
{
    public class UserImage
    {
        public Guid Id { get; set; }

        public byte[] Image { get; set; }

        public virtual User User { get; set; }
    }
}
