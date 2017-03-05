// http://stackoverflow.com/questions/13334938/automapper-flattening-of-nested-mappings-asks-for-a-custom-resolver
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Fido.Action.Models.Account;
using Fido.Core;
using Fido.Core.Bootstrapper;

namespace Fido.Action.Mapping
{
    class SettingsMapping : AutoMapper.Profile, IBootstrapper
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
                Mapper.CreateMap<Dtos.Settings, Settings>()
                    .ForMember(Dest => Dest.ExternalCredentials, Options => Options.MapFrom(Src => Mapper.Map<IList<Dtos.ExternalCredential>, IList<ExternalCredential>>(Src.ExternalCredentials)))
                    .ForMember(Dest => Dest.Id, Options => Options.Ignore())
                    .ForMember(Dest => Dest.CreatedUtc, Options => Options.Ignore())
                    .ForMember(Dest => Dest.CreatedAgeDays, Options => Options.Ignore())
                    .ForMember(Dest => Dest.IsNew, Options => Options.Ignore())
                    .ForMember(Dest => Dest.RowVersion, Options => Options.Ignore())
                    .ForMember(Dest => Dest.ReadAccess, Options => Options.Ignore())
                    .ForMember(Dest => Dest.WriteAccess, Options => Options.Ignore())
                    .ForMember(Dest => Dest.FeedbackAPI, Options => Options.Ignore())
                    .ForMember(Dest => Dest.AuthenticationAPI, Options => Options.Ignore())
                    .ForMember(Dest => Dest.ModelAPI, Options => Options.Ignore())
                    .ForMember(Dest => Dest.Denied, Options => Options.Ignore());
            }
        }
    }
}
