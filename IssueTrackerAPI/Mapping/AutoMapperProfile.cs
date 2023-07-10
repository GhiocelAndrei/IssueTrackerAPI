using AutoMapper;
using IssueTrackerAPI.Models;

namespace IssueTrackerAPI.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Issue, IssueDto>();
            CreateMap<Project, ProjectDto>();
            CreateMap<User, UserDto>();
        }
    }
}
