using Moq;
using AutoMapper;
using IssueTracker.Application.Services;
using IssueTracker.DataAccess.Repositories;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Enums;
using IssueTracker.Abstractions.Mapping;

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
            _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(projects);

            // Act
            var result = await _sut.GetAll();

            // Assert
            _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task Get_ById_ShoudReturnNull_WhenProjectNotFound()
        {
            // Arrange
            var id = 1;
            _mockRepository.Setup(r => r.GetAsync(id)).ReturnsAsync((Project)null);

            // Act
            var result = await _sut.Get(id);

            // Assert
            Assert.Null(result);
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

            _mockRepository.Setup(x => x.GetAsync(projectId)).ReturnsAsync(project);

            // Act
            var result = await _sut.Get(projectId);

            // Assert
            _mockRepository.Verify(x => x.GetAsync(projectId), Times.Once);
            Assert.Equal(project, result);
        }

        [Fact]
        public async Task Update_ShouldReturnNull_WhenProjectNotFound()
        {
            // Arrange
            var id = 1;
            var command = new UpdateProjectCommand();

            _mockRepository.Setup(r => r.GetAsync(id)).ReturnsAsync((Project)null);

            // Act
            var result = await _sut.Update(id, command);

            // Assert
            Assert.Null(result);
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

            _mockRepository.Setup(r => r.GetAsync(1)).ReturnsAsync(project);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Project>())).ReturnsAsync((Project p) => p);

            // Act
            var result = await _sut.Update(1, updateProjectCommand);

            // Assert
            _mockRepository.Verify(r => r.GetAsync(1), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(project), Times.Once);
            
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

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Project>())).ReturnsAsync(expectedProject);

            // Act
            var result = await _sut.Create(createProjectCommand);

            // Assert
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Project>()), Times.Once);

            Assert.Equal(createProjectCommand.Name, result.Name);
        }

        [Fact]
        public async Task Delete_ShouldCallDeleteAsyncOnRepository()
        {
            // Arrange
            var projectId = 1;
            var deletedProject = new Project();

            _mockRepository.Setup(r => r.GetAsync(projectId)).ReturnsAsync(deletedProject);
            _mockRepository.Setup(r => r.DeleteAsync(projectId)).ReturnsAsync(deletedProject);

            // Act
            await _sut.Delete(projectId);

            // Assert
            _mockRepository.Verify(r => r.DeleteAsync(projectId), Times.Once);
        }
    }
}
