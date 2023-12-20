using Application.Activities;
using AutoMapper;
using Domain;

namespace Application.Core
{
    // Class defining AutoMapper profiles: MappingProfiles
    public class MappingProfiles : Profile
    {
         // Configure the AutoMapper profile
        public MappingProfiles()
        {
            // Mapping Activity to Activity
            CreateMap<Activity, Activity>();
            // Mapping Activity to ActivityDto
            CreateMap<Activity, ActivityDto>()
                .ForMember(d => d.HostUsername, o => o.MapFrom(s => s.Attendees.FirstOrDefault(x => x.IsHost).AppUser.UserName));
            CreateMap<ActivityAttendee, Profiles.Profile>()
                .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.AppUser.DisplayName))
                .ForMember(d => d.Username, o => o.MapFrom(s => s.AppUser.UserName))
                .ForMember(d => d.Bio, o => o.MapFrom(s => s.AppUser.Bio));
        }
    }
}