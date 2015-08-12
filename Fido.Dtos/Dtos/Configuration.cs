using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Dtos
{
    public class Configuration : Dto
    {
        public int PasswordChangePolicyDays { get; set; }
        public bool PasswordChangePolicy { get; set; }
    }
}
