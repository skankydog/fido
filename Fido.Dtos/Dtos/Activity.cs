using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Dtos
{
    public class Activity : Dto
    {
        public Activity()
            : base()
        {
        }

        public string Name { get; set; }

        public List<Role> Roles { get; set; } 
    }
}