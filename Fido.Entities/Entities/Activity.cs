using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities
{
    public class Activity : Entity
    {
        public Activity()
        {
            Roles = new List<Role>();
        }

        public string Action { get; set; }
        public string Name { get; set; }
        public string Area { get; set; }

        public ICollection<Role> Roles { get; set; }
    }
}
