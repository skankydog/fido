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
    class ProfileMapping : AutoMapper.Profile, IBootstrapper
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
                Mapper.CreateMap<Dtos.Profile, Fido.Action.Models.Profile>()
                    .ForMember(Dest => Dest.IsNew, Options => Options.UseValue(false)) // Viewmodel created from a read
                    .ForMember(Dest => Dest.Firstname, Options => Options.MapFrom(Src => Src.Fullname.Firstname))
                    .ForMember(Dest => Dest.Surname, Options => Options.MapFrom(Src => Src.Fullname.Surname))
                    .ForMember(Dest => Dest.FirstnameSurname, Options => Options.MapFrom(Src => Src.Fullname.FirstnameSurname))
                    .ForMember(Dest => Dest.RequiresReadPermission, Options => Options.Ignore());

                Mapper.CreateMap<Fido.Action.Models.Profile, Dtos.Fullname>()
                    .ForMember(Dest => Dest.FirstnameSurname, Options => Options.Ignore())
                    .ForMember(Dest => Dest.SurnameFirstname, Options => Options.Ignore());
                Mapper.CreateMap<Fido.Action.Models.Profile, Dtos.Profile>()
                    .ForMember(Dest => Dest.Fullname, Options => Options.MapFrom(Src => Mapper.Map<Fido.Action.Models.Profile, Dtos.Fullname>(Src)));
            }
        }
    }
}
