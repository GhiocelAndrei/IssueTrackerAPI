using AutoMapper;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Mapping;

namespace IssueTracker.Application.Services
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
