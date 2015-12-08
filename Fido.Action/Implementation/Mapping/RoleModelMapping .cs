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
    class RoleModelMapping : Profile, IBootstrapper
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
                Mapper.CreateMap<Dtos.Role, Guid>()
                    .ConvertUsing(Src => Src.Id);
                Mapper.CreateMap<Dtos.Role, RoleModel>()
                    .ForMember(Dest => Dest.RequiresAuthentication, Options => Options.Ignore());

                Mapper.CreateMap<RoleModel, Dtos.Role>();
            }
        }
    }
}
