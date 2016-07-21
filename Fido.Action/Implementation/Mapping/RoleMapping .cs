// http://stackoverflow.com/questions/13334938/automapper-flattening-of-nested-mappings-asks-for-a-custom-resolver
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Fido.Action.Models.Administration;
using Fido.Core;
using Fido.Core.Bootstrapper;

namespace Fido.Action.Mapping
{
    class RoleMapping : AutoMapper.Profile, IBootstrapper
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
                Mapper.CreateMap<Dtos.Role, Guid>()
                    .ConvertUsing(Src => Src.Id);
                Mapper.CreateMap<Dtos.Role, Role>()
                    .ForMember(Dest => Dest.IsNew, Options => Options.UseValue(false)) // Viewmodel created from a read
                    .ForMember(Dest => Dest.AllActivities, Options => Options.Ignore())
                    .ForMember(Dest => Dest.AllUsers, Options => Options.Ignore())
                    .ForMember(Dest => Dest.SelectedActivities, Options => Options.MapFrom(Src => Mapper.Map<IList<Dtos.Activity>, IList<Guid>>(Src.Activities)))
                    .ForMember(Dest => Dest.SelectedUsers, Options => Options.MapFrom(Src => Mapper.Map<IList<Dtos.User>, IList<Guid>>(Src.Users)))
                    .ForMember(Dest => Dest.ReadAccess, Options => Options.Ignore())
                    .ForMember(Dest => Dest.WriteAccess, Options => Options.Ignore());

                Mapper.CreateMap<Role, Dtos.Role>()
                    .ForMember(Dest => Dest.Activities, Options => Options.Ignore())
                    .ForMember(Dest => Dest.Users, Options => Options.Ignore());
            }
        }
    }
}
