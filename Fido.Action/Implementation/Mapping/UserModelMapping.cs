﻿// http://stackoverflow.com/questions/13334938/automapper-flattening-of-nested-mappings-asks-for-a-custom-resolver
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
    class UserModelMapping : Profile, IBootstrapper
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
                Mapper.CreateMap<Dtos.User, UserModel>()
                    .ForMember(Dest => Dest.IsNew, Options => Options.UseValue(false)) // Viewmodel created from a read
                    //.ForMember(Dest => Dest.AllLocalCredentialStates, Options => Options.MapFrom(Src => new HashSet<string>() { "Expired", "Enabled", "Disabled", Src.LocalCredentialState }))
                    //.ForMember(Dest => Dest.AllExternalCredentialStates, Options => Options.MapFrom(Src => new HashSet<string>() { "Enabled", "Disabled", Src.ExternalCredentialState }))
                    .ForMember(Dest => Dest.AllLocalCredentialStates, Options => Options.Ignore())
                    .ForMember(Dest => Dest.AllExternalCredentialStates, Options => Options.Ignore())
                    .ForMember(Dest => Dest.AllRoles, Options => Options.Ignore())
                    .ForMember(Dest => Dest.Firstname, Options => Options.MapFrom(Src => Src.Fullname.Firstname))
                    .ForMember(Dest => Dest.Surname, Options => Options.MapFrom(Src => Src.Fullname.Surname))
                    .ForMember(Dest => Dest.DisplayName, Options => Options.MapFrom(Src => Src.Fullname.DisplayName))
                    .ForMember(Dest => Dest.HasFacebook, Options => Options.MapFrom(Src => Src.HasLoginProvider("facebook")))
                    .ForMember(Dest => Dest.HasTwitter, Options => Options.MapFrom(Src => Src.HasLoginProvider("twitter")))
                    .ForMember(Dest => Dest.HasLinkedIn, Options => Options.MapFrom(Src => Src.HasLoginProvider("linkedin")))
                    .ForMember(Dest => Dest.HasGoogle, Options => Options.MapFrom(Src => Src.HasLoginProvider("google")))
                    .ForMember(Dest => Dest.SelectedRoles, Options => Options.MapFrom(Src => Mapper.Map<IList<Dtos.Role>, IList<Guid>>(Src.Roles)))
                    .ForMember(Dest => Dest.RequiresReadPermission, Options => Options.Ignore());

                Mapper.CreateMap<UserModel, Dtos.Fullname>()
                    .ForMember(Dest => Dest.DisplayName, Options => Options.Ignore());
                Mapper.CreateMap<UserModel, Dtos.User>()
                    .ForMember(Dest => Dest.LocalCredentialState, Options => Options.MapFrom(m => m.ExternalCredentialState == null ? "None" : m.ExternalCredentialState))
                    .ForMember(Dest => Dest.Fullname, Options => Options.MapFrom(Src => Mapper.Map<UserModel, Dtos.Fullname>(Src)))
                    .ForMember(Dest => Dest.ExternalCredentials, Options => Options.Ignore()) // Can't be updated via this view
                 //   .ForMember(Dest => Dest.Roles, Options => Options.UseValue(Src => (from Src.SelectedRoles select new Dtos.Role { Id=  Src.SelectedRoles })));
                    .ForMember(Dest => Dest.Roles, Options => Options.Ignore());
            }
        }
    }
}
