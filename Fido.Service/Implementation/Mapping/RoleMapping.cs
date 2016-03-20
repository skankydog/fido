// http://stackoverflow.com/questions/13334938/automapper-flattening-of-nested-mappings-asks-for-a-custom-resolver

//ConfigurationStore store = new ConfigurationStore(new TypeMapFactory(), MapperRegistry.AllMappers());
//store.AssertConfigurationIsValid();
//MappingEngine engine = new MappingEngine(store);
//
////add mappings via Profiles or CreateMapstore.AddProfile<MyAutoMapperProfile>();
//store.CreateMap<Dto.Ticket, Entities.Ticket>();

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Fido.Core;
using Fido.Core.Bootstrapper;

namespace Fido.Service.Mapping
{
    class RoleMapping : Profile, IBootstrapper
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
                Mapper.CreateMap<Entities.Role, Dtos.Role>()
                    .ForMember(Dest => Dest.IsNew, Options => Options.UseValue(false)); // Dto was created from a read

                Mapper.CreateMap<Dtos.Role, Entities.Role>()
                    .ForMember(Dest => Dest.Activities, Options => Options.MapFrom(Src => Mapper.Map<IList<Dtos.Activity>, IList<Entities.Activity>>(Src.Activities)))
                    .ForMember(Dest => Dest.Users, Options => Options.MapFrom(Src => Mapper.Map<IList<Dtos.User>, IList<Entities.User>>(Src.Users)));
            }
        }
    }
}
