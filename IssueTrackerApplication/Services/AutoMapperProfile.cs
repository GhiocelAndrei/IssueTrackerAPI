using AutoMapper;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Mapping;

namespace IssueTracker.Application.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Issue Mappings
            CreateMap<Issue, IssueDto>();

            CreateMap<Issue, CreateIssueCommand>();
            CreateMap<CreateIssueCommand, Issue>();
            CreateMap<IssueCreatingDto, CreateIssueCommand>();
            CreateMap<IssueCreatingDto, Issue>();

            CreateMap<Issue, UpdateIssueCommand>();
            CreateMap<UpdateIssueCommand, Issue>();
            CreateMap<IssueUpdatingDto, UpdateIssueCommand>();
            CreateMap<IssueUpdatingDto, Issue>();

            // Project Mapping
            CreateMap<Project, ProjectDto>();

            CreateMap<Project, CreateProjectCommand>();
            CreateMap<CreateProjectCommand, Project>();
            CreateMap<ProjectCreatingDto, CreateProjectCommand>();
            CreateMap<ProjectCreatingDto, Project>();

            CreateMap<Project, UpdateProjectCommand>();
            CreateMap<UpdateProjectCommand, Project>();
            CreateMap<ProjectUpdatingDto, UpdateProjectCommand>();
            CreateMap<ProjectUpdatingDto, Project>();

            // User Mapping
            CreateMap<User, UserDto>();

            CreateMap<User, CreateUserCommand>();
            CreateMap<CreateUserCommand, User>();
            CreateMap<UserCreatingDto, CreateUserCommand>();
            CreateMap<UserCreatingDto, User>();

            CreateMap<User, UpdateUserCommand>();
            CreateMap<UpdateUserCommand, User>();
            CreateMap<UserUpdatingDto, UpdateUserCommand>();
            CreateMap<UserUpdatingDto, User>();

            CreateMap<UserLoginDto, CreateUserCommand>();
        }
    }
}
