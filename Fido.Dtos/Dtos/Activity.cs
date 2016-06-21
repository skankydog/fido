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
        public string Area { get; set; }
        public string Action { get; set; }

        public IList<Role> Roles { get; set; }
    }
}