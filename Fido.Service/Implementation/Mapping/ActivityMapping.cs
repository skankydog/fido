// http://stackoverflow.com/questions/13334938/automapper-flattening-of-nested-mappings-asks-for-a-custom-resolver

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Fido.Core;
using Fido.Core.Bootstrapper;

namespace Fido.Service.Mapping
{
    class ActivityMapping : Profile, IBootstrapper
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
                Mapper.CreateMap<Entities.Activity, Dtos.Activity>()
                    .ForMember(Dest => Dest.IsNew, Options => Options.UseValue(false)); // Dto was created from a read

                Mapper.CreateMap<Dtos.Activity, Entities.Activity>()
                    .ForMember(Dest => Dest.Roles, Options => Options.Ignore());
            }
        }
    }
}
