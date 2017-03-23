// http://stackoverflow.com/questions/13334938/automapper-flattening-of-nested-mappings-asks-for-a-custom-resolver
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Fido.ViewModel.Models.Account;
using Fido.Core;
using Fido.Core.Bootstrapper;

namespace Fido.ViewModel.Mapping
{
    class ExternalCredentialMapping : AutoMapper.Profile, IBootstrapper
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
                Mapper.CreateMap<Dtos.ExternalCredential, ExternalCredential>()
                    .ForMember(Dest => Dest.ReadAccess, Options => Options.Ignore())
                    .ForMember(Dest => Dest.WriteAccess, Options => Options.Ignore())
                    .ForMember(Dest => Dest.FeedbackAPI, Options => Options.Ignore())
                    .ForMember(Dest => Dest.AuthenticationAPI, Options => Options.Ignore())
                    .ForMember(Dest => Dest.ModelAPI, Options => Options.Ignore())
                    .ForMember(Dest => Dest.Denied, Options => Options.Ignore());

                Mapper.CreateMap<ExternalCredential, Dtos.ExternalCredential>()
                    .ForMember(Dest => Dest.CreatedUtc, Options => Options.Ignore())
                    .ForMember(Dest => Dest.CreatedAgeDays, Options => Options.Ignore())
                    .ForMember(Dest => Dest.IsNew, Options => Options.Ignore())
                    .ForMember(Dest => Dest.RowVersion, Options => Options.Ignore());
            }
        }
    }
}
