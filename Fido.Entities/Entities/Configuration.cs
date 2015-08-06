using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities
{
    public class Configuration : Entity
    {
        public int PasswordChangePolicyDays { get; set; }
    }
}
