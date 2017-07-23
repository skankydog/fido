// http://stackoverflow.com/questions/13334938/automapper-flattening-of-nested-mappings-asks-for-a-custom-resolver
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
    class ActivityMapping : AutoMapper.Profile, IBootstrapper
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
                Mapper.CreateMap<Dtos.Activity, Guid>()
                    .ConvertUsing(Src => Src.Id);
                Mapper.CreateMap<Dtos.Activity, Activity>()
                    .ForMember(Dest => Dest.IsNew, Options => Options.UseValue(false)) // Viewmodel created from a read
                    .ForMember(Dest => Dest.SelectedRoles, Options => Options.MapFrom(Src => Mapper.Map<IList<Dtos.Role>, IList<Guid>>(Src.Roles)))
                    .ForMember(Dest => Dest.ReadAccess, Options => Options.Ignore())
                    .ForMember(Dest => Dest.WriteAccess, Options => Options.Ignore())
                    .ForMember(Dest => Dest.FeedbackAPI, Options => Options.Ignore())
                    .ForMember(Dest => Dest.AuthenticationAPI, Options => Options.Ignore())
                    .ForMember(Dest => Dest.ModelAPI, Options => Options.Ignore())
                    .ForMember(Dest => Dest.Denied, Options => Options.Ignore())
                    .ForMember(Dest => Dest.AllRoles, Options => Options.Ignore());

                Mapper.CreateMap<Activity, Dtos.Activity>()
                    .ForMember(Dest => Dest.Roles, Options => Options.Ignore())
                    .ForMember(Dest => Dest.Name, Options => Options.Ignore())
                    .ForMember(Dest => Dest.Area, Options => Options.Ignore())
                    .ForMember(Dest => Dest.ReadWrite, Options => Options.Ignore());
            }
        }
    }
}
