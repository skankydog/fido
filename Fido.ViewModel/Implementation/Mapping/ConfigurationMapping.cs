﻿// http://stackoverflow.com/questions/13334938/automapper-flattening-of-nested-mappings-asks-for-a-custom-resolver
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Fido.ViewModel.Models.Administration;
using Fido.Core;
using Fido.Core.Bootstrapper;

namespace Fido.ViewModel.Mapping
{
    class ConfigurationMapping : AutoMapper.Profile, IBootstrapper
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
                Mapper.CreateMap<Dtos.Configuration, Configuration>()
                    .ForMember(Dest => Dest.IsNew, Options => Options.UseValue(false)) // Viewmodel created from a read
                    .ForMember(Dest => Dest.ReadAccess, Options => Options.Ignore())
                    .ForMember(Dest => Dest.WriteAccess, Options => Options.Ignore())
                    .ForMember(Dest => Dest.FeedbackAPI, Options => Options.Ignore())
                    .ForMember(Dest => Dest.AuthenticationAPI, Options => Options.Ignore())
                    .ForMember(Dest => Dest.ModelAPI, Options => Options.Ignore())
                    .ForMember(Dest => Dest.Denied, Options => Options.Ignore());

                Mapper.CreateMap<Configuration, Dtos.Configuration>();
            }
        }
    }
}
