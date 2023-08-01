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
            CreateMap<long?, long>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<string?, string>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<Priority?, Priority>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<bool?, bool>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<DateTime?, DateTime>().ConvertUsing((src, dest) => src ?? dest);

            // Issue Mappings
            CreateMap<Issue, IssueDto>();

            CreateMap<CreateIssueCommand, Issue>();
            CreateMap<IssueCreatingDto, CreateIssueCommand>();
            CreateMap<IssueCreatingDto, Issue>();

            CreateMap<UpdateIssueCommand, Issue>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<IssueUpdatingDto, UpdateIssueCommand>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Project Mapping
            CreateMap<Project, ProjectDto>();

            CreateMap<CreateProjectCommand, Project>();
            CreateMap<ProjectCreatingDto, CreateProjectCommand>();
            CreateMap<ProjectCreatingDto, Project>();

            CreateMap<UpdateProjectCommand, Project>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<ProjectUpdatingDto, UpdateProjectCommand>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // User Mapping
            CreateMap<User, UserDto>();

            CreateMap<CreateUserCommand, User>();
            CreateMap<UserCreatingDto, CreateUserCommand>();
            CreateMap<UserCreatingDto, User>();

            CreateMap<UpdateUserCommand, User>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UserUpdatingDto, UpdateUserCommand>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UserLoginDto, LoginUserCommand>();

            // Sprint Mapping
            CreateMap<Sprint, SprintDto>();

            CreateMap<CreateSprintCommand, Sprint>();
            CreateMap<SprintCreatingDto, CreateSprintCommand>();
            CreateMap<SprintCreatingDto, Sprint>();

            CreateMap<UpdateSprintCommand, Sprint>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<SprintUpdatingDto, UpdateSprintCommand>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<SprintCreatingWithIssuesDto, CreateSprintWithIssuesCommand>();
        }
    }
}
