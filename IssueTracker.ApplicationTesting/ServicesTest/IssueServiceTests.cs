using Moq;
using AutoMapper;
using IssueTracker.Application.Services;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Enums;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Exceptions;
using IssueTracker.DataAccess.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using FluentValidation;
using Newtonsoft.Json;
using FluentValidation.Results;

namespace IssueTracker.Testing.ServicesTest
{
    public class IssueServiceTests : IDisposable
    {
        private IssueContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IssuesService _sut;
        private readonly Mock<IUsersService> _userServiceMock;
        private readonly Mock<IProjectsService> _projectServiceMock;
        private readonly Mock<IValidatorFactory> _validatorFactoryMock;

        public IssueServiceTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<IssueContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            _dbContext = new IssueContext(optionsBuilder.Options);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            _mapper = mapperConfig.CreateMapper();

            _userServiceMock = new Mock<IUsersService>();
            _projectServiceMock = new Mock<IProjectsService>();
            _validatorFactoryMock = new Mock<IValidatorFactory>();

            _sut = new IssuesService(_dbContext, _mapper, _validatorFactoryMock.Object, _userServiceMock.Object, _projectServiceMock.Object);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            _dbContext = null;
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllIssues()
        {
            // Arrange
            _dbContext.Issues.Add(new Issue());
            _dbContext.Issues.Add(new Issue());
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _sut.GetAllAsync(It.IsAny<CancellationToken>());

            // Assert
            Assert.Equal(2, result.Count());
        }


        [Fact]
        public async Task Get_ById_ShouldThrowNotFoundException_WhenIssueNotFound()
        {
            // Arrange
            var id = 1;

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await _sut.GetAsync(id, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Get_ById_ShouldReturnIssue()
        {
            // Arrange
            var issuesId = 1;
            var issue = new Issue { Id = issuesId };
            _dbContext.Issues.Add(issue);
            await _dbContext.SaveChangesAsync();

            // Act
            var returnedIssue = await _sut.GetAsync(issuesId, It.IsAny<CancellationToken>());
            
            // Assert
            Assert.Equal(issuesId, returnedIssue.Id);
        }

        [Fact]
        public async Task Update_ShouldThrowNotFoundException_WhenIssueNotFound()
        {
            // Arrange
            var id = 1;
            var command = new UpdateIssueCommand();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await _sut.UpdateAsync(id, command, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Update_ShouldUpdateAndReturnUpdatedIssue()
        {
            // Arrange
            var id = 1;

            var issue = new Issue
            {
                Id = id,
                Title = "Old title",
                Description = "Old description",
                Priority = Priority.High,
                ReporterId = 1,
                AssigneeId = 1,
                ProjectId = 1
            };

            var updateIssueCommand = new UpdateIssueCommand
            {
                Title = "New title",
                Description = "New description",
                Priority = Priority.Low,
                ReporterId = 2,
                AssigneeId = 2,
                ProjectId = 2
            };

            _dbContext.Issues.Add(issue);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _sut.UpdateAsync(id, updateIssueCommand, It.IsAny<CancellationToken>());

            // Assert
            Assert.Equal(issue, result);
            Assert.Equal(updateIssueCommand.Title, result.Title);
            Assert.Equal(updateIssueCommand.Description, result.Description);
            Assert.Equal(updateIssueCommand.Priority, result.Priority);
            Assert.Equal(updateIssueCommand.ReporterId, result.ReporterId);
            Assert.Equal(updateIssueCommand.AssigneeId, result.AssigneeId);
            Assert.Equal(updateIssueCommand.ProjectId, result.ProjectId);
        }

        [Fact]
        public async Task Create_ShouldThrow_WhenNonExistingProject()
        {
            // Arrange
            var createIssueCommand = new CreateIssueCommand();

            _projectServiceMock.Setup(p => p.ExistsAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _userServiceMock.Setup(p => p.ExistsAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidInputException>(async () =>
                await _sut.CreateAsync(createIssueCommand, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Create_ShouldThrow_WhenNonExistingUser()
        {
            // Arrange
            var createIssueCommand = new CreateIssueCommand();

            _projectServiceMock.Setup(p => p.ExistsAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _userServiceMock.Setup(p => p.ExistsAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidInputException>(async () =>
                await _sut.CreateAsync(createIssueCommand, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Create_ShouldCreateAndReturnNewIssue()
        {
            // Arrange
            var createIssueCommand = new CreateIssueCommand
            {
                Title = "New issue",
                Description = "Issue description",
                Priority = Priority.Low,
                ReporterId = 2,
                AssigneeId = 2,
                ProjectId = 2
            };

            var expectedIssue = _mapper.Map<Issue>(createIssueCommand);
            expectedIssue.Id = 1;

            var reporter = new User
            {
                Id = 2
            };
            _dbContext.Users.Add(reporter);
            await _dbContext.SaveChangesAsync();

            _projectServiceMock.Setup(p => p.ExistsAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _userServiceMock.Setup(p => p.ExistsAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _projectServiceMock.Setup(p => p.GetIssueSequenceAsync(createIssueCommand.ProjectId, It.IsAny<CancellationToken>())).ReturnsAsync(123);
            _projectServiceMock.Setup(p => p.GetProjectCodeAsync(createIssueCommand.ProjectId, It.IsAny<CancellationToken>())).ReturnsAsync("PROJ");
            // Act
            var result = await _sut.CreateAsync(createIssueCommand, It.IsAny<CancellationToken>());

            // Assert
            Assert.Equal(expectedIssue.Id, result.Id);
            Assert.Equal("PROJ-123", result.ExternalId);
            Assert.Equal(expectedIssue.Title, result.Title);
            Assert.Equal(expectedIssue.Description, result.Description);
            Assert.Equal(expectedIssue.Priority, result.Priority);
            Assert.Equal(expectedIssue.ReporterId, result.ReporterId);
            Assert.Equal(expectedIssue.AssigneeId, result.AssigneeId);
            Assert.Equal(expectedIssue.ProjectId, result.ProjectId);
        }

        [Fact]
        public async Task Delete_ShouldDeleteAndReturnDeletedIssue()
        {
            // Arrange
            var issueId = 1;
            var deletedIssue = new Issue { Id = issueId };
            _dbContext.Issues.Add(deletedIssue);
            await _dbContext.SaveChangesAsync();

            // Act
            await _sut.DeleteAsync(issueId, It.IsAny<CancellationToken>());

            // Assert
            Assert.False(_dbContext.Issues.Any(issue => issue.Id == issueId));
        }

        [Fact]
        public async Task AssignSprintToIssuesAsync_ShouldThrowInvalidInputException_WhenIssueWithIdNotFound()
        {
            // Arrange
            var sprintId = 1;

            // Act & Assert
            await Assert.ThrowsAsync<InvalidInputException>(async () =>
                await _sut.AssignSprintToIssuesAsync(new List<long> { 1 }, sprintId, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task AssignSprintToIssuesAsync_ShouldAssignSprintToIssues()
        {
            // Arrange
            var sprintId = 1;

            // Create some issues in the database
            var issue1 = new Issue { Id = 1 };
            var issue2 = new Issue { Id = 2 };
            var issue3 = new Issue { Id = 3 };
            _dbContext.Issues.AddRange(issue1, issue2, issue3);
            await _dbContext.SaveChangesAsync();

            // Convert ids to long (assuming they are actually long)
            var idsToAssign = new List<long> { 1, 2 };

            // Act
            await _sut.AssignSprintToIssuesAsync(idsToAssign, sprintId, It.IsAny<CancellationToken>());

            // Assert
            var updatedIssue1 = await _dbContext.Issues.FindAsync(1L);
            var updatedIssue2 = await _dbContext.Issues.FindAsync(2L);
            var unchangedIssue3 = await _dbContext.Issues.FindAsync(3L);

            Assert.Equal(sprintId, updatedIssue1.SprintId);
            Assert.Equal(sprintId, updatedIssue2.SprintId);
            Assert.Null(unchangedIssue3.SprintId);
        }

        [Fact]
        public async Task PatchAsync_ShouldThrowNotFoundException_WhenIssueNotFound()
        {
            // Arrange
            var id = 1;
            var jsonPatchDocument = new JsonPatchDocument<IssueUpdatingDto>();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await _sut.PatchAsync(id, jsonPatchDocument, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task PatchAsync_ShouldThrowInvalidInputException_WhenInvalidPathInPatchDocument()
        {
            // Arrange
            var issueId = 1;
            var issue = new Issue { Id = issueId };

            _dbContext.Issues.Add(issue);
            await _dbContext.SaveChangesAsync();

            string jsonPatch = @"
            [
                {
                    ""op"": ""replace"",
                    ""path"": ""/CreatedAt"",
                    ""value"": ""2023-07-31T15:00:03.42""
                }
            ]";

            var patchDoc = JsonConvert.DeserializeObject<JsonPatchDocument<IssueUpdatingDto>>(jsonPatch);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidInputException>(() => _sut.PatchAsync(issueId, patchDoc, default));
        }

        [Fact]
        public async Task PatchAsync_ShouldThrowsInvalidInputException_WhenInvalidValueInPatchDocument()
        {
            // Arrange
            var issueId = 1;
            var issue = new Issue { Id = issueId };

            _dbContext.Issues.Add(issue);
            await _dbContext.SaveChangesAsync();

            var patchDoc = new JsonPatchDocument<IssueUpdatingDto>();
            patchDoc.Replace(i => i.Title, null);

            var mockValidator = new Mock<IValidator<IssueUpdatingDto>>();
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Title", "Title cannot be null") });
            mockValidator.Setup(v => v.ValidateAsync(It.IsAny<IssueUpdatingDto>(), default)).ReturnsAsync(validationResult);

            _validatorFactoryMock.Setup(f => f.GetValidator<IssueUpdatingDto>()).Returns(mockValidator.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidInputException>(() => _sut.PatchAsync(issueId, patchDoc, default));
        }

        [Fact]
        public async Task PatchAsync_ShouldApplyPatchToIssueAndSaveChanges()
        {
            // Arrange
            var id = 1L;
            var issue = new Issue
            {
                Id = id,
                Title = "Old title",
                Description = "Old description",
                Priority = Priority.High,
                ReporterId = 1,
                AssigneeId = 1,
                ProjectId = 1
            };

            _dbContext.Issues.Add(issue);
            await _dbContext.SaveChangesAsync();

            var patchedTitle = "Patched title";
            var patchedDescription = "Patched description";

            var jsonPatchDocument = new JsonPatchDocument<IssueUpdatingDto>();
            jsonPatchDocument.Replace(x => x.Title, patchedTitle);
            jsonPatchDocument.Replace(x => x.Description, patchedDescription);

            var mockValidator = new Mock<IValidator<IssueUpdatingDto>>();
            mockValidator.Setup(v => v.ValidateAsync(It.IsAny<IssueUpdatingDto>(), default)).ReturnsAsync(new ValidationResult());

            _validatorFactoryMock.Setup(f => f.GetValidator<IssueUpdatingDto>()).Returns(mockValidator.Object);
            // Act
            await _sut.PatchAsync(id, jsonPatchDocument, CancellationToken.None);
            var updatedIssue = await _dbContext.Issues.FindAsync(id);

            // Assert
            Assert.Equal(issue.Id, updatedIssue.Id);
            Assert.Equal(patchedTitle, updatedIssue.Title);
            Assert.Equal(patchedDescription, updatedIssue.Description);
            Assert.Equal(issue.Priority, updatedIssue.Priority);
            Assert.Equal(issue.ReporterId, updatedIssue.ReporterId);
            Assert.Equal(issue.AssigneeId, updatedIssue.AssigneeId);
            Assert.Equal(issue.ProjectId, updatedIssue.ProjectId);
        }
    }
}
