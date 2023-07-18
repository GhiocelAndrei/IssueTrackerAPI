using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace IssueTracker.Application.Services
{
    public class ProjectService
    {
        private readonly IGenericRepository<Project> _projectRepository;
        private readonly IMapper _mapper;

        public ProjectService(IGenericRepository<Project> projectRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Project>> GetProjects()
        {
            var projects = await _projectRepository.GetAll();

            return projects;
        }

        public async Task<Project> GetProject(long id)
        {
            var project = await _projectRepository.Get(id);

            return project;
        }

        public async Task<Project> PutProject(long id, ProjectCommandDto projectCommand)
        {
            var projectModified = await _projectRepository.Get(id);

            if (projectModified == null)
            {
                return null;
            }

            _mapper.Map(projectCommand, projectModified);

            try
            {
                await _projectRepository.Update(projectModified);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (projectModified == null)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }

            return projectModified;
        }

        public async Task<Project> PostProject(ProjectCommandDto projectCommand)
        {
            var project = _mapper.Map<Project>(projectCommand);

            var createdProject = await _projectRepository.Add(project);

            return createdProject;
        }

        public async Task DeleteProject(long id)
        {
            await _projectRepository.Delete(id);
        }
    }
}
