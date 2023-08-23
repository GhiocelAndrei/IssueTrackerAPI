using Moq;
using AutoMapper;
using IssueTracker.Application.Services;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Exceptions;
using IssueTracker.DataAccess.DatabaseContext;
using IssueTracker.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using FluentValidation;
using Newtonsoft.Json;
using FluentValidation.Results;

namespace IssueTracker.Testing.ServicesTest
{
    public class ProjectServiceTests : IDisposable
    {
        private IssueContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ProjectsService _sut;
        private readonly Mock<IValidatorFactory> _validatorFactoryMock;
        private readonly Mock<IProjectRepository> projectRepositoryMock;
        
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

            _validatorFactoryMock = new Mock<IValidatorFactory>();
            projectRepositoryMock = new Mock<IProjectRepository>();

            _sut = new ProjectsService(_dbContext, _mapper, projectRepositoryMock.Object, _validatorFactoryMock.Object);
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
           await _dbContext.SaveChangesAsync();

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
           await _dbContext.SaveChangesAsync();

            // Act
            var returnedProject = await _sut.GetAsync(projectId, It.IsAny<CancellationToken>());

            // Assert
            Assert.Equal(projectId, returnedProject.Id);
        }

        [Fact]
        public async Task Update_ShouldThrowNotFoundException_WhenProjectNotFound()
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
           await _dbContext.SaveChangesAsync();

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

            var expectedProject = _mapper.Map<Project>(createProjectCommand);
            expectedProject.Id = 1;

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

        [Fact]
        public async Task PatchAsync_ShouldThrowNotFoundException_WhenProjectNotFound()
        {
            // Arrange
            var id = 1;
            var jsonPatchDocument = new JsonPatchDocument<ProjectUpdatingDto>();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await _sut.PatchAsync(id, jsonPatchDocument, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task PatchAsync_ShouldThrowInvalidInputException_WhenInvalidPathInPatchDocument()
        {
            // Arrange
            var projectId = 1;
            var project = new Project { Id = projectId };

            _dbContext.Projects.Add(project);
            await _dbContext.SaveChangesAsync();

            string jsonPatch = @"
            [
                {
                    ""op"": ""replace"",
                    ""path"": ""/CreatedAt"",
                    ""value"": ""2023-07-31T15:00:03.42""
                }
            ]";

            var patchDoc = JsonConvert.DeserializeObject<JsonPatchDocument<ProjectUpdatingDto>>(jsonPatch);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidInputException>(() => _sut.PatchAsync(projectId, patchDoc, default));
        }

        [Fact]
        public async Task PatchAsync_ShouldThrowsInvalidInputException_WhenInvalidValueInPatchDocument()
        {
            // Arrange
            var projectId = 1;
            var project = new Project { Id = projectId };

            _dbContext.Projects.Add(project);
            await _dbContext.SaveChangesAsync();

            var patchDoc = new JsonPatchDocument<ProjectUpdatingDto>();
            patchDoc.Replace(i => i.Name, null);

            var mockValidator = new Mock<IValidator<ProjectUpdatingDto>>();
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be null") });
            mockValidator.Setup(v => v.ValidateAsync(It.IsAny<ProjectUpdatingDto>(), default)).ReturnsAsync(validationResult);

            _validatorFactoryMock.Setup(f => f.GetValidator<ProjectUpdatingDto>()).Returns(mockValidator.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidInputException>(() => _sut.PatchAsync(projectId, patchDoc, default));
        }

        [Fact]
        public async Task PatchAsync_ShouldApplyPatchToProjectAndSaveChanges()
        {
            // Arrange
            var id = 1L;
            var project = new Project
            {
                Id = id,
                Name = "Old Name"
            };

            _dbContext.Projects.Add(project);
            await _dbContext.SaveChangesAsync();

            var patchedName = "Patched Name";

            var jsonPatchDocument = new JsonPatchDocument<ProjectUpdatingDto>();
            jsonPatchDocument.Replace(x => x.Name, patchedName);

            var mockValidator = new Mock<IValidator<ProjectUpdatingDto>>();
            mockValidator.Setup(v => v.ValidateAsync(It.IsAny<ProjectUpdatingDto>(), default)).ReturnsAsync(new ValidationResult());

            _validatorFactoryMock.Setup(f => f.GetValidator<ProjectUpdatingDto>()).Returns(mockValidator.Object);
            // Act
            await _sut.PatchAsync(id, jsonPatchDocument, CancellationToken.None);
            var updatedProject = await _dbContext.Projects.FindAsync(id);

            // Assert
            Assert.Equal(project.Id, updatedProject.Id);
            Assert.Equal(patchedName, updatedProject.Name);
        }

        [Fact]
        public async Task SearchAsync_ShouldThrowInvalidInputException_WhenPropertyDoesNotExist()
        {
            // Arrange, Act & Assert
            await Assert.ThrowsAsync<InvalidInputException>(() => _sut.SearchAsync("NonExistentProperty", "SomeValue", 50, CancellationToken.None));
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnEmptyList_WhenNoMatches()
        {
            // Arrange
            var project = new Project { Name = "TestProject" };
            _dbContext.Projects.Add(project);
            await _dbContext.SaveChangesAsync();

            // Act
            var results = await _sut.SearchAsync("Name", "NonMatchingValue", 50, CancellationToken.None);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnProjectsList_WhenMatching()
        {
            // Arrange
            var project1 = new Project { Name = "SameName" };
            var project2 = new Project { Name = "SameName" };
            var project3 = new Project { Name = "DifferentName" };
            _dbContext.Projects.Add(project1);
            _dbContext.Projects.Add(project2);
            _dbContext.Projects.Add(project3);
            await _dbContext.SaveChangesAsync();

            var projectsList = new List<Project> { project1, project2 };

            // Act
            var results = await _sut.SearchAsync("Name", "SameName", 50, CancellationToken.None);

            // Assert
            Assert.Equal(projectsList, results);
        }
    }
}
