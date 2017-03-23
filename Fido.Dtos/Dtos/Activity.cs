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

        public string Area { get; set; }
        public string Name { get; set; }
        public string ReadWrite { get; set; }
        public string FullQualification { get { return string.Concat(Area, ".", Name, ".", ReadWrite); } }

        public IList<Role> Roles { get; set; }
    }
}
