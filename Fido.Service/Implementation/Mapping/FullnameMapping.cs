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
    class FullnameMapping : Profile, IBootstrapper
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
                Mapper.CreateMap<Entities.UserDetails.Fullname, Dtos.Fullname>()
                    .ForMember(Dest => Dest.DisplayName, Options => Options.MapFrom(Src => string.Concat(Src.Firstname, " ", Src.Surname)));

                Mapper.CreateMap<Dtos.Fullname, Entities.UserDetails.Fullname>();
            }
        }
    }
}
