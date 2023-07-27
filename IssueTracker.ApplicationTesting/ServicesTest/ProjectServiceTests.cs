using Moq;
using AutoMapper;
using IssueTracker.Application.Services;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Exceptions;
using IssueTracker.DataAccess.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Testing.ServicesTest
{
    public class ProjectServiceTests : IDisposable
    {
        private IssueContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ProjectsService _sut;

        public ProjectServiceTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<IssueContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            _dbContext = new IssueContext(optionsBuilder.Options);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            _mapper = mapperConfig.CreateMapper();

            _sut = new ProjectsService(_dbContext, _mapper);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            _dbContext = null;
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllProjects()
        {
            // Arrange
            _dbContext.Projects.Add(new Project());
            _dbContext.Projects.Add(new Project());
            _dbContext.SaveChanges();

            // Act
            var result = await _sut.GetAllAsync(It.IsAny<CancellationToken>());

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task Get_ById_ShouldThrowNotFoundException_WhenProjectNotFound()
        {
            // Arrange
            var id = 1;

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await _sut.GetAsync(id, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Get_ById_ShouldReturnProject()
        {
            // Arrange
            var projectId = 1;
            var project = new Project { Id = projectId };
            _dbContext.Projects.Add(project);
            _dbContext.SaveChanges();

            // Act
            var returnedProject = await _sut.GetAsync(projectId, It.IsAny<CancellationToken>());

            // Assert
            Assert.Equal(projectId, returnedProject.Id);
        }

        [Fact]
        public async Task Update_ShouldThrowNotFoundException_WhenIssueNotFound()
        {
            // Arrange
            var id = 1;
            var command = new UpdateProjectCommand();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await _sut.UpdateAsync(id, command, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Update_ShouldUpdateAndReturnUpdatedProject()
        {
            // Arrange
            var id = 1;

            var project = new Project
            {
                Id = id,
                Name = "Old Project"
            };

            var updateProjectCommand = new UpdateProjectCommand
            {
                Name = "New Project"
            };

            _dbContext.Projects.Add(project);
            _dbContext.SaveChanges();

            // Act
            var result = await _sut.UpdateAsync(id, updateProjectCommand, It.IsAny<CancellationToken>());

            // Assert
            Assert.Equal(updateProjectCommand.Name, result.Name);
        }

        [Fact]
        public async Task Create_ShouldCreateAndReturnNewProject()
        {
            // Arrange
            var createProjectCommand = new CreateProjectCommand
            {
                Name = "Test Project"
            };

            var expectedIssue = _mapper.Map<Project>(createProjectCommand);
            expectedIssue.Id = 1;

            // Act
            var result = await _sut.CreateAsync(createProjectCommand, It.IsAny<CancellationToken>());

            // Assert
            Assert.Equal(createProjectCommand.Name, result.Name);
        }

        [Fact]
        public async Task Delete_ShouldCallDeleteAsyncOnRepository()
        {
            // Arrange
            var projectId = 1;
            var deletedProject = new Project { Id = projectId };
            _dbContext.Projects.Add(deletedProject);
            await _dbContext.SaveChangesAsync();

            // Act
            await _sut.DeleteAsync(projectId, It.IsAny<CancellationToken>());

            // Assert
            Assert.False(_dbContext.Issues.Any(project => project.Id == projectId));
        }
    }
}
