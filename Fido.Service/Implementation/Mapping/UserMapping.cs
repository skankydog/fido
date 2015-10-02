// http://stackoverflow.com/questions/13334938/automapper-flattening-of-nested-mappings-asks-for-a-custom-resolver
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Fido.Core;
using Fido.Core.Bootstrapper;

namespace Fido.Service.Mapping
{
    class UserMapping : Profile, IBootstrapper
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
                Mapper.CreateMap<Entities.User, Dtos.User>()
                    .ForMember(Dest => Dest.IsNew, Options => Options.UseValue(false)) // Dto was created from a read
                    .ForMember(Dest => Dest.HasLocalCredentials, Options => Options.MapFrom(Src => Src.CurrentLocalCredentialState.HasCredentials))
                    .ForMember(Dest => Dest.PasswordAgeDays, Options => Options.MapFrom(Src => Src.PasswordLastChangeUtc == null ? (int?)null : Convert.ToInt16((DateTime.UtcNow - (DateTime)Src.PasswordLastChangeUtc).TotalDays)))
                    .ForMember(Dest => Dest.EmailAddressAgeDays, Options => Options.MapFrom(Src => Src.EmailAddressLastChangeUtc == null ? (int?)null : Convert.ToInt16((DateTime.UtcNow - (DateTime)Src.EmailAddressLastChangeUtc).TotalDays)))
                    .ForMember(Dest => Dest.CreatedAgeDays, Options => Options.MapFrom(Src => Src.CreatedUtc == null ? (int?)null : Convert.ToInt16((DateTime.UtcNow - (DateTime)Src.CreatedUtc).TotalDays)))
                    .ForMember(Dest => Dest.HasExternalCredentials, Options => Options.MapFrom(Src => Src.CurrentExternalCredentialState.HasCredentials));

                Mapper.CreateMap<Dtos.User, Entities.User>()
                    .ForMember(Dest => Dest.EmailAddress, Options => Options.Ignore()) // Changes to EmailAddress must be done via service call
                    .ForMember(Dest => Dest.Password, Options => Options.Ignore()) // Not available to the DTO
                    .ForMember(Dest => Dest.LocalCredentialState, Options => Options.Ignore()) // Read only to the DTO
                    .ForMember(Dest => Dest.CurrentLocalCredentialState, Options => Options.Ignore()) // Read only to the DTO
                    .ForMember(Dest => Dest.PasswordLastChangeUtc, Options => Options.Ignore()) // Read only to the DTO
                    .ForMember(Dest => Dest.EmailAddressLastChangeUtc, Options => Options.Ignore()) // Read only to the DTO
                    .ForMember(Dest => Dest.ExternalCredentialState, Options => Options.Ignore()) // Read only to the DTO
                    .ForMember(Dest => Dest.CurrentExternalCredentialState, Options => Options.Ignore()) // Read only to the DTO
                    .ForMember(Dest => Dest.Roles, Options => Options.Ignore())
                    .ForMember(Dest => Dest.ExternalCredentials, Options => Options.Ignore())
                    .ForMember(Dest => Dest.ProfileImage, Options => Options.Ignore());
            }
        }
    }
}
