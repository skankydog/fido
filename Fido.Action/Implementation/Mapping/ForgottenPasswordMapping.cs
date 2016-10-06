// http://stackoverflow.com/questions/13334938/automapper-flattening-of-nested-mappings-asks-for-a-custom-resolver
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Fido.Action.Models.Authentication;
using Fido.Core;
using Fido.Core.Bootstrapper;

namespace Fido.Action.Mapping
{
    class ForgottenPasswordMapping : AutoMapper.Profile, IBootstrapper
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
                Mapper.CreateMap<Dtos.Confirmation, ResetPassword>()
                    .ForMember(Dest => Dest.ConfirmationId, Options => Options.MapFrom(Src => Src.Id))
                    .ForMember(Dest => Dest.Password, Options => Options.Ignore())
                    .ForMember(Dest => Dest.ConfirmPassword, Options => Options.Ignore())
                    .ForMember(Dest => Dest.ReadAccess, Options => Options.Ignore())
                    .ForMember(Dest => Dest.WriteAccess, Options => Options.Ignore())
                    .ForMember(Dest => Dest.FeedbackAPI, Options => Options.Ignore())
                    .ForMember(Dest => Dest.AuthenticationAPI, Options => Options.Ignore())
                    .ForMember(Dest => Dest.ModelAPI, Options => Options.Ignore())
                    .ForMember(Dest => Dest.DeniedActivities, Options => Options.Ignore());
            }
        }
    }
}
