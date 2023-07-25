using Moq;
using AutoMapper;
using IssueTracker.Application.Services;
using IssueTracker.DataAccess.Repositories;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Exceptions;

namespace IssueTracker.Testing.ServicesTest
{
    public class ProjectServiceTests
    {
        private readonly Mock<IGenericRepository<Project>> _mockRepository;
        private readonly IMapper _mapper;
        private readonly ProjectService _sut;

        public ProjectServiceTests()
        {
            _mockRepository = new Mock<IGenericRepository<Project>>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            _mapper = mapperConfig.CreateMapper();

            _sut = new ProjectService(_mockRepository.Object, _mapper);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllProjects()
        {
            // Arrange
            var projects = new List<Project> { new Project(), new Project() };
            _mockRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(projects);

            // Act
            var result = await _sut.GetAllAsync(It.IsAny<CancellationToken>());

            // Assert
            _mockRepository.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task Get_ById_ShouldThrowNotFoundException_WhenProjectNotFound()
        {
            // Arrange
            var id = 1;
            _mockRepository.Setup(r => r.GetAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Project)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await _sut.GetAsync(id, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Get_ById_ShouldReturnProject()
        {
            // Arrange
            var projectId = 1;

            var project = new Project 
            { 
                Id = projectId 
            };

            _mockRepository.Setup(x => x.GetAsync(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(project);

            // Act
            var result = await _sut.GetAsync(projectId, It.IsAny<CancellationToken>());

            // Assert
            _mockRepository.Verify(x => x.GetAsync(projectId, It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(project, result);
        }

        [Fact]
        public async Task Update_ShouldThrowNotFoundException_WhenIssueNotFound()
        {
            // Arrange
            var id = 1;
            var command = new UpdateProjectCommand();
            _mockRepository.Setup(r => r.GetAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Project)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await _sut.UpdateAsync(id, command, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Update_ShouldUpdateAndReturnUpdatedProject()
        {
            // Arrange
            var project = new Project
            {
                Id = 1,
                Name = "Old Project"
            };

            var updateProjectCommand = new UpdateProjectCommand
            {
                Name = "New Project"
            };

            _mockRepository.Setup(r => r.GetAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(project);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
                            .ReturnsAsync((Project p, CancellationToken cancellationToken) => p);

            // Act
            var result = await _sut.UpdateAsync(1, updateProjectCommand, It.IsAny<CancellationToken>());

            // Assert
            _mockRepository.Verify(r => r.GetAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(project, It.IsAny<CancellationToken>()), Times.Once);
            
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

            var expectedProject = _mapper.Map<Project>(createProjectCommand);
            expectedProject.Id = 1;

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedProject);

            // Act
            var result = await _sut.CreateAsync(createProjectCommand, It.IsAny<CancellationToken>());

            // Assert
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Once);

            Assert.Equal(createProjectCommand.Name, result.Name);
        }

        [Fact]
        public async Task Delete_ShouldCallDeleteAsyncOnRepository()
        {
            // Arrange
            var projectId = 1;
            var deletedProject = new Project();

            _mockRepository.Setup(r => r.GetAsync(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(deletedProject);
            _mockRepository.Setup(r => r.DeleteAsync(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(deletedProject);

            // Act
            await _sut.DeleteAsync(projectId, It.IsAny<CancellationToken>());

            // Assert
            _mockRepository.Verify(r => r.DeleteAsync(projectId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
