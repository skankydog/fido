// http://stackoverflow.com/questions/13334938/automapper-flattening-of-nested-mappings-asks-for-a-custom-resolver
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Fido.ViewModel.Models;
using Fido.Core;
using Fido.Core.Bootstrapper;

namespace Fido.ViewModel.Mapping
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
                Mapper.CreateMap<Dtos.Profile, Fido.ViewModel.Models.Account.Profile>()
                    .ForMember(Dest => Dest.IsNew, Options => Options.UseValue(false)) // Viewmodel created from a read
                    .ForMember(Dest => Dest.Firstname, Options => Options.MapFrom(Src => Src.Fullname.Firstname))
                    .ForMember(Dest => Dest.Surname, Options => Options.MapFrom(Src => Src.Fullname.Surname))
                    .ForMember(Dest => Dest.FirstnameSurname, Options => Options.MapFrom(Src => Src.Fullname.FirstnameSurname))
                    .ForMember(Dest => Dest.ReadAccess, Options => Options.Ignore())
                    .ForMember(Dest => Dest.WriteAccess, Options => Options.Ignore())
                    .ForMember(Dest => Dest.FeedbackAPI, Options => Options.Ignore())
                    .ForMember(Dest => Dest.AuthenticationAPI, Options => Options.Ignore())
                    .ForMember(Dest => Dest.ModelAPI, Options => Options.Ignore())
                    .ForMember(Dest => Dest.Denied, Options => Options.Ignore());

                Mapper.CreateMap<Fido.ViewModel.Models.Account.Profile, Dtos.Fullname>()
                    .ForMember(Dest => Dest.FirstnameSurname, Options => Options.Ignore())
                    .ForMember(Dest => Dest.SurnameFirstname, Options => Options.Ignore());
                Mapper.CreateMap<Fido.ViewModel.Models.Account.Profile, Dtos.Profile>()
                    .ForMember(Dest => Dest.Fullname, Options => Options.MapFrom(Src => Mapper.Map<Fido.ViewModel.Models.Account.Profile, Dtos.Fullname>(Src)));
            }
        }
    }
}
