using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Dtos
{
    public class Profile : Dto
    {
        public string EmailAddress { get; set; }
        public Fullname Fullname { get; set; }
        public string About { get; set; }
        public byte[] Image { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int RegisteredDays { get; set; }
    }
}
