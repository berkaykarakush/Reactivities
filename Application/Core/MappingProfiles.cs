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
            // Mapping Activity to Activity and reverse 
            CreateMap<Activity, Activity>().ReverseMap();
        }
    }
}