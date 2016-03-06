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
    class SettingsVMMapping : Profile, IBootstrapper
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
                Mapper.CreateMap<Dtos.Settings, SettingsVM>()
                    .ForMember(Dest => Dest.ExternalCredentials, Options => Options.MapFrom(Src => Mapper.Map<IList<Dtos.ExternalCredential>, IList<ExternalCredentialVM>>(Src.ExternalCredentials)))
                    .ForMember(Dest => Dest.RequiresReadPermission, Options => Options.Ignore());
            }
        }
    }
}
