using AutoMapper;
using IssueTrackerAPI.Models;

namespace IssueTrackerAPI.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Issue, IssueDto>();
            CreateMap<IssueCreatingDto, Issue>();

            CreateMap<Project, ProjectDto>();
            CreateMap<ProjectCreatingDto, Project>();

            CreateMap<User, UserDto>();
            CreateMap<UserCreatingDto, User>();
        }
    }
}
