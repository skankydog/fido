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
        public IList<Activity> Activities { get; set; }
        public IList<User> Users { get; set; }
    }
}
