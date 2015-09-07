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
                Mapper.CreateMap<Entities.UserDetails.Fullname, Dtos.Fullname>()
                    .ForMember(Dest => Dest.DisplayName, Options => Options.MapFrom(Src => string.Concat(Src.Firstname, " ", Src.Surname)));
                Mapper.CreateMap<Entities.ProfileImage, Dtos.Profile>()
                    .ForMember(Dest => Dest.About, Options => Options.Ignore())
                    .ForMember(Dest => Dest.CreatedUtc, Options => Options.Ignore())
                    .ForMember(Dest => Dest.DateOfBirth, Options => Options.Ignore())
                    .ForMember(Dest => Dest.EmailAddress, Options => Options.Ignore())
                    .ForMember(Dest => Dest.IsNew, Options => Options.Ignore())
                 //   .ForMember(Dest => Dest.CreatedAgeDays, Options => Options.Ignore())
                    .ForMember(Dest => Dest.Fullname, Options => Options.Ignore());
                Mapper.CreateMap<Entities.User, Dtos.Profile>()
                    .ForMember(Dest => Dest.IsNew, Options => Options.UseValue(false)) // Dto was created from a read
                    .ForMember(Dest => Dest.DateOfBirth, Options => Options.Ignore())
                 //   .ForMember(Dest => Dest.RegisteredDays, Options => Options.MapFrom(Src => int.Parse(Math.Truncate((DateTime.UtcNow - Src.CreatedUtc).TotalDays).ToString())))
                    .ForMember(Dest => Dest.Image, Options => Options.Ignore());

                Mapper.CreateMap<Dtos.Profile, Entities.ProfileImage>()
                    .ForMember(Dest => Dest.User, Options => Options.Ignore())
                    .ForMember(Dest => Dest.UserId, Options => Options.MapFrom(Src => Src.Id));
                Mapper.CreateMap<Dtos.Fullname, Entities.UserDetails.Fullname>()
                    .ForMember(Dest => Dest.DisplayName, Options => Options.Ignore());
                Mapper.CreateMap<Dtos.Profile, Entities.User>()
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
                    .ForMember(Dest => Dest.ProfileImage, Options => Options.Ignore())
                    .ForMember(Dest => Dest.CreatedAgeDays, Options => Options.Ignore())
                    .ForMember(Dest => Dest.EmailAddressAgeDays, Options => Options.Ignore())
              //      .ForMember(Dest => Dest.PasswordAgeDays, Options => Options.Ignore())
                    .ForMember(Dest => Dest.Fullname, Options => Options.MapFrom(Src => Mapper.Map<Dtos.Profile, Entities.UserDetails.Fullname>(Src)));
            }
        }
    }
}
