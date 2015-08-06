using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Fido.Core;
using Fido.Dtos;
using Fido.Service.Implementation;

namespace Fido.Service
{
    public interface IProfileImageService
    {
        ProfileImage Download(Guid Id);
      //  void Upload(ProfileImage UserImage);
    }
}
