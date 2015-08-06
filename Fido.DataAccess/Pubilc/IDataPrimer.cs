using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.DataAccess
{
    public interface IDataPrimer
    {
        void Refresh();
        void Delete();
    }
}
