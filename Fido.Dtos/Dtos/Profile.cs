using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Dtos
{
    public class Profile : Dto
    {
        public string EmailAddress { get; set; } // Read only
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string DisplayName { get; set; } // Read only
        public string About { get; set; }
        public byte[] Image { get; set; } // Write only
        public DateTime DateOfBirth { get; set; }
        public int RegisteredDays { get; set; } // Read only
    }
}
