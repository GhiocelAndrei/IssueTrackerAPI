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
            CreateMap<Issue, IssueCommandDto>();
            CreateMap<IssueCommandDto, Issue>();
            CreateMap<IssueCreatingDto, IssueCommandDto>();
            CreateMap<IssueCreatingDto, Issue>();

            CreateMap<Project, ProjectDto>();
            CreateMap<Project, ProjectCommandDto>();
            CreateMap<ProjectCommandDto, Project>();
            CreateMap<ProjectCreatingDto, ProjectCommandDto>();
            CreateMap<ProjectCreatingDto, Project>();

            CreateMap<User, UserDto>();
            CreateMap<User, UserCommandDto>();
            CreateMap<UserCommandDto, User>();
            CreateMap<UserCreatingDto, UserCommandDto>();
            CreateMap<UserCreatingDto, User>();
        }
    }
}
