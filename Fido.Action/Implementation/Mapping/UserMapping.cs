// http://stackoverflow.com/questions/13334938/automapper-flattening-of-nested-mappings-asks-for-a-custom-resolver
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Fido.Action.Models;
using Fido.Core;
using Fido.Core.Bootstrapper;

namespace Fido.Action.Mapping
{
    class UserMapping : Profile, IBootstrapper
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Initialise()
        {
            Mapper.AddProfile(this);
        }

        protected override void Configure()
        {
            using (new FunctionLogger(Log))
            {
                Mapper.CreateMap<Dtos.Fullname, UserModel>();
                Mapper.CreateMap<Dtos.User, UserModel>()
                    .ForMember(Dest => Dest.IsNew, Options => Options.UseValue(false)); // Viewmodel created from a read

                Mapper.CreateMap<UserModel, Dtos.Fullname>()
                    .ForMember(Dest => Dest.DisplayName, Options => Options.Ignore());
                Mapper.CreateMap<UserModel, Dtos.User>();
            }
        }
    }
}
