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
                    .ForMember(Dest => Dest.IsNew, Options => Options.UseValue(false)); // Dto was created from a read

                Mapper.CreateMap<Dtos.User, Entities.User>()
                    .ForMember(Dest => Dest.EmailAddress, Options => Options.Ignore()) // Changes to EmailAddress must be done via service call
                    .ForMember(Dest => Dest.Password, Options => Options.Ignore()) // Not available to the DTO
                    .ForMember(Dest => Dest.LocalCredentialState, Options => Options.Ignore()) // Read only to the DTO
                    .ForMember(Dest => Dest.CurrentLocalCredentialState, Options => Options.Ignore()) // Read only to the DTO
                    .ForMember(Dest => Dest.PasswordLastChangeUtc, Options => Options.Ignore()) // Read only to the DTO
                    .ForMember(Dest => Dest.EmailAddressLastChangeUtc, Options => Options.Ignore()) // Read only to the DTO
                    .ForMember(Dest => Dest.ExternalCredentialState, Options => Options.Ignore()) // Read only to the DTO
                    .ForMember(Dest => Dest.CurrentExternalCredentialState, Options => Options.Ignore()) // Read only to the DTO
                    .ForMember(Dest => Dest.UserImage, Options => Options.MapFrom(Src => Mapper.Map<Dtos.User, Entities.UserImage>(Src)))
                    .ForMember(Dest => Dest.Roles, Options => Options.MapFrom(Src => Mapper.Map<IList<Dtos.Role>, IList<Entities.Role>>(Src.Roles)))
                    .ForMember(Dest => Dest.ExternalCredentials, Options => Options.Ignore())
                    .ForMember(Dest => Dest.UserImage, Options => Options.Ignore());
            }
        }
    }
}
