using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities.UserDetails
{
    public class Fullname
    {
        public string Firstname { get; set; }
        public string Surname { get; set; }

        public string FirstnameSurname { get { return string.Concat(Firstname, " ", Surname); } }
        public string SurnameFirstname { get { return string.Concat(Surname, ", ", Firstname); } }
    }
}
