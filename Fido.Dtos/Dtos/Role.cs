﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Dtos
{
    public class Role : Dto
    {
        public Role()
            : base()
        {
        }

        public string Name { get; set; }

        public IList<Activity> Activities { get; set; }
        public IList<User> Users { get; set; }
    }
}
