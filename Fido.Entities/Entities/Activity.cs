using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities
{
    public enum Permission
    {
        Read = 0,
        ReadWrite
    }

    public class Activity : Entity
    {
        public Activity()
        {
            Roles = new List<Role>();
        }

        public string Name { get; set; }

        public ICollection<Role> Roles { get; set; }
    }
}
