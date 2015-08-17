using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fido.Dtos;

namespace Fido.Service
{
    public interface ICRUDService<TDTO>
        where TDTO : Dto
    {
        TDTO Get(Guid Id, string IncludeProperties = "");
        IList<TDTO> GetAll();
        TDTO Save(TDTO DTO, string IncludeProperties = "");
        bool Delete(Guid Id);
    }
}
