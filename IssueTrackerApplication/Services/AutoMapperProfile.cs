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

            CreateMap<CreateIssueCommand, Issue>();
            CreateMap<IssueCreatingDto, CreateIssueCommand>();
            CreateMap<IssueCreatingDto, Issue>();

            CreateMap<UpdateIssueCommand, Issue>();
            CreateMap<IssueUpdatingDto, UpdateIssueCommand>();
            CreateMap<IssueUpdatingDto, Issue>();

            // Project Mapping
            CreateMap<Project, ProjectDto>();

            CreateMap<CreateProjectCommand, Project>();
            CreateMap<ProjectCreatingDto, CreateProjectCommand>();
            CreateMap<ProjectCreatingDto, Project>();

            CreateMap<UpdateProjectCommand, Project>();
            CreateMap<ProjectUpdatingDto, UpdateProjectCommand>();
            CreateMap<ProjectUpdatingDto, Project>();

            // User Mapping
            CreateMap<User, UserDto>();

            CreateMap<CreateUserCommand, User>();
            CreateMap<UserCreatingDto, CreateUserCommand>();
            CreateMap<UserCreatingDto, User>();

            CreateMap<UpdateUserCommand, User>();
            CreateMap<UserUpdatingDto, UpdateUserCommand>();
            CreateMap<UserUpdatingDto, User>();

            CreateMap<UserLoginDto, LoginUserCommand>();

            // Sprint Mapping
            CreateMap<Sprint, SprintDto>();

            CreateMap<CreateSprintCommand, Sprint>();
            CreateMap<SprintCreatingDto, CreateSprintCommand>();
            CreateMap<SprintCreatingDto, Sprint>();

            CreateMap<UpdateSprintCommand, Sprint>();
            CreateMap<SprintUpdatingDto, UpdateSprintCommand>();
            CreateMap<SprintUpdatingDto, Sprint>();

            CreateMap<SprintCreatingWithIssuesDto, CreateSprintWithIssuesCommand>();
        }
    }
}
