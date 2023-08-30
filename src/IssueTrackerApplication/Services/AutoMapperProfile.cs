using AutoMapper;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Enums;

namespace IssueTracker.Application.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Issue Mappings
            CreateMap<Issue, IssueDto>();

            CreateMap<CreateIssueCommand, Issue>();
            CreateMap<IssueCreatingDto, CreateIssueCommand>();
            CreateMap<IssueCreatingDto, Issue>();

            CreateMap<Issue, IssueUpdatingDto>();
            CreateMap<IssueUpdatingDto, Issue>();
            CreateMap<UpdateIssueCommand, Issue>();
            CreateMap<IssueUpdatingDto, UpdateIssueCommand>();

            // Project Mapping
            CreateMap<Project, ProjectDto>();

            CreateMap<CreateProjectCommand, Project>();
            CreateMap<ProjectCreatingDto, CreateProjectCommand>();
            CreateMap<ProjectCreatingDto, Project>();

            CreateMap<Project, ProjectUpdatingDto>();
            CreateMap<ProjectUpdatingDto, Project>();
            CreateMap<UpdateProjectCommand, Project>();
            CreateMap<ProjectUpdatingDto, UpdateProjectCommand>();

            // User Mapping
            CreateMap<User, UserDto>();

            CreateMap<CreateUserCommand, User>();
            CreateMap<UserCreatingDto, CreateUserCommand>();
            CreateMap<UserCreatingDto, User>();

            CreateMap<User, UserUpdatingDto>();
            CreateMap<UserUpdatingDto, User>();
            CreateMap<UpdateUserCommand, User>();
            CreateMap<UserUpdatingDto, UpdateUserCommand>();

            CreateMap<UserLoginDto, LoginUserCommand>();

            // Sprint Mapping
            CreateMap<Sprint, SprintDto>();

            CreateMap<CreateSprintCommand, Sprint>();
            CreateMap<SprintCreatingDto, CreateSprintCommand>();
            CreateMap<SprintCreatingDto, Sprint>();

            CreateMap<Sprint, SprintUpdatingDto>();
            CreateMap<SprintUpdatingDto, Sprint>();
            CreateMap<UpdateSprintCommand, Sprint>();
            CreateMap<SprintUpdatingDto, UpdateSprintCommand>();

            CreateMap<SprintCreatingWithIssuesDto, CreateSprintWithIssuesCommand>();
        }
    }
}
