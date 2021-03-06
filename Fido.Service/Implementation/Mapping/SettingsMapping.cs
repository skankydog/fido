﻿// http://stackoverflow.com/questions/13334938/automapper-flattening-of-nested-mappings-asks-for-a-custom-resolver
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Fido.Core;
using Fido.Core.Bootstrapper;

namespace Fido.Service.Mapping
{
    class SettingsMapping : Profile, IBootstrapper
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
                Mapper.CreateMap<Entities.User, Dtos.Settings>()
                    .ForMember(Dest => Dest.PasswordChangePolicy, Options => Options.Ignore())
                    .ForMember(Dest => Dest.PasswordChangePolicyDays, Options => Options.Ignore());
                Mapper.CreateMap<Entities.Configuration, Dtos.Settings>()
                    .ForMember(Dest => Dest.EmailAddress, Options => Options.Ignore())
                    .ForMember(Dest => Dest.Fullname, Options => Options.Ignore())
                    .ForMember(Dest => Dest.LocalCredentialsArePresent, Options => Options.Ignore())
                    .ForMember(Dest => Dest.LocalCredentialsAreUsable, Options => Options.Ignore())
                    .ForMember(Dest => Dest.LocalCredentialState, Options => Options.Ignore())
                    .ForMember(Dest => Dest.ExternalCredentialsArePresent, Options => Options.Ignore())
                    .ForMember(Dest => Dest.ExternalCredentialState, Options => Options.Ignore())
                    .ForMember(Dest => Dest.ExternalCredentials, Options => Options.Ignore())
                    .ForMember(Dest => Dest.PasswordAgeDays, Options => Options.Ignore());
            }
        }
    }
}
