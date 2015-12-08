using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities
{
    public class Role : Entity
    {
        public Role()
        {
            Activities = new List<Activity>();
            Users = new List<User>();
        }

        public string Name { get; set; }
        public ICollection<Activity> Activities { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
