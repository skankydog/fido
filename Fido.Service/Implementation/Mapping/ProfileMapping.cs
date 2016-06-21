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
    class ProfileMapping : Profile, IBootstrapper
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
                Mapper.CreateMap<Entities.UserImage, byte[]>()
                    .ConvertUsing(Src => Src.Image);
                Mapper.CreateMap<Entities.User, Dtos.Profile>()
                    .ForMember(Dest => Dest.IsNew, Options => Options.UseValue(false)) // Dto was created from a read
                    .ForMember(Dest => Dest.Image, Options =>
                        Options.MapFrom(Src => Src.UserImage == null ? null : Mapper.Map<Entities.UserImage, byte[]>(Src.UserImage)))
                    .ForMember(Dest => Dest.DateOfBirth, Options => Options.Ignore()); // TO DO: DoB needed later

                Mapper.CreateMap<Dtos.Profile, Entities.UserImage>()
                    .ForMember(Dest => Dest.Image, Options => Options.MapFrom(Src => Src.Image))
                    .ForMember(Dest => Dest.User, Options => Options.Ignore());
                Mapper.CreateMap<Dtos.Profile, Entities.User>()
                    .ForMember(Dest => Dest.UserImage, Options => Options.MapFrom(Src => Mapper.Map<Dtos.Profile, Entities.UserImage>(Src)))
                    .ForMember(Dest => Dest.Roles, Options => Options.Ignore()) // Don't know
                    .ForMember(Dest => Dest.ExternalCredentials, Options => Options.Ignore()) // Don't know
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
                    .ForMember(Dest => Dest.EmailAddressAgeDays, Options => Options.Ignore());
            }
        }
    }
}
