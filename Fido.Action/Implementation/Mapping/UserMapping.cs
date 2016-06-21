// http://stackoverflow.com/questions/13334938/automapper-flattening-of-nested-mappings-asks-for-a-custom-resolver
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Fido.Action.Models.Administration;
using Fido.Core;
using Fido.Core.Bootstrapper;

namespace Fido.Action.Mapping
{
    class UserMapping : AutoMapper.Profile, IBootstrapper
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
                Mapper.CreateMap<Dtos.User, Guid>()
                    .ConvertUsing(Src => Src.Id);
                Mapper.CreateMap<Dtos.User, User>()
                    .ForMember(Dest => Dest.IsNew, Options => Options.UseValue(false)) // Viewmodel created from a read
                    .ForMember(Dest => Dest.AllLocalCredentialStates, Options => Options.Ignore())
                    .ForMember(Dest => Dest.AllExternalCredentialStates, Options => Options.Ignore())
                    .ForMember(Dest => Dest.AllRoles, Options => Options.Ignore())
                    .ForMember(Dest => Dest.Firstname, Options => Options.MapFrom(Src => Src.Fullname.Firstname))
                    .ForMember(Dest => Dest.Surname, Options => Options.MapFrom(Src => Src.Fullname.Surname))
                    .ForMember(Dest => Dest.FirstnameSurname, Options => Options.MapFrom(Src => Src.Fullname.FirstnameSurname))
                    .ForMember(Dest => Dest.SurnameFirstname, Options => Options.MapFrom(Src => Src.Fullname.SurnameFirstname))
                    .ForMember(Dest => Dest.HasFacebook, Options => Options.MapFrom(Src => Src.HasLoginProvider("facebook")))
                    .ForMember(Dest => Dest.HasTwitter, Options => Options.MapFrom(Src => Src.HasLoginProvider("twitter")))
                    .ForMember(Dest => Dest.HasLinkedIn, Options => Options.MapFrom(Src => Src.HasLoginProvider("linkedin")))
                    .ForMember(Dest => Dest.HasGoogle, Options => Options.MapFrom(Src => Src.HasLoginProvider("google")))
                    .ForMember(Dest => Dest.SelectedRoles, Options => Options.MapFrom(Src => Mapper.Map<IList<Dtos.Role>, IList<Guid>>(Src.Roles)))
                    .ForMember(Dest => Dest.RequiresReadPermission, Options => Options.Ignore());

                Mapper.CreateMap<User, Dtos.Fullname>()
                    .ForMember(Dest => Dest.FirstnameSurname, Options => Options.Ignore())
                    .ForMember(Dest => Dest.SurnameFirstname, Options => Options.Ignore());
                Mapper.CreateMap<User, Dtos.User>()
                    .ForMember(Dest => Dest.LocalCredentialState, Options => Options.MapFrom(m => m.LocalCredentialState == null ? "None" : m.LocalCredentialState))
                    .ForMember(Dest => Dest.ExternalCredentialState, Options => Options.MapFrom(m => m.ExternalCredentialState == null ? "None" : m.ExternalCredentialState))
                    .ForMember(Dest => Dest.Fullname, Options => Options.MapFrom(Src => Mapper.Map<User, Dtos.Fullname>(Src)))
                    .ForMember(Dest => Dest.ExternalCredentials, Options => Options.Ignore()) // Can't be updated via this view
                    .ForMember(Dest => Dest.Roles, Options => Options.Ignore());
            }
        }
    }
}
