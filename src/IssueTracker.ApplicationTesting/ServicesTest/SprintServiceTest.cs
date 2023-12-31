﻿using Moq;
using AutoMapper;
using IssueTracker.Application.Services;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Exceptions;
using IssueTracker.DataAccess.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using FluentValidation;
using Newtonsoft.Json;
using FluentValidation.Results;
using System.Transactions;

namespace IssueTracker.ApplicationTesting.ServicesTest
{
    public class SprintServiceTests : IDisposable
    {
        private IssueContext _dbContext;
        private readonly IMapper _mapper;
        private readonly SprintsService _sut;
        private readonly Mock<IIssuesService> _issuesServiceMock;
        private readonly Mock<IValidatorFactory> _validatorFactoryMock;
        private readonly Mock<IUnitOfWork> _transactionUnitMock;

        public SprintServiceTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<IssueContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            _dbContext = new IssueContext(optionsBuilder.Options);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            _mapper = mapperConfig.CreateMapper();

            _issuesServiceMock = new Mock<IIssuesService>();
            _validatorFactoryMock = new Mock<IValidatorFactory>();
            _transactionUnitMock = new Mock<IUnitOfWork>();

            _sut = new SprintsService(_dbContext, _mapper, _validatorFactoryMock.Object, _issuesServiceMock.Object, _transactionUnitMock.Object);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            _dbContext = null;
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllSprints()
        {
            // Arrange
            _dbContext.Sprints.Add(new Sprint());
            _dbContext.Sprints.Add(new Sprint());
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _sut.GetAllAsync(It.IsAny<CancellationToken>());

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task Get_ById_ShouldThrowNotFoundException_WhenSprintNotFound()
        {
            // Arrange
            var id = 1;

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await _sut.GetAsync(id, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Get_ById_ShouldReturnSprint()
        {
            // Arrange
            var sprintId = 1;
            var sprint = new Sprint { Id = sprintId };
            _dbContext.Sprints.Add(sprint);
            await _dbContext.SaveChangesAsync();

            // Act
            var returnedSprint = await _sut.GetAsync(sprintId, It.IsAny<CancellationToken>());

            // Assert
            Assert.Equal(sprintId, returnedSprint.Id);
        }

        [Fact]
        public async Task Update_ShouldThrowNotFoundException_WhenSprintNotFound()
        {
            // Arrange
            var id = 1;
            var command = new UpdateSprintCommand();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await _sut.UpdateAsync(id, command, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Update_ShouldUpdateAndReturnUpdatedSprint()
        {
            // Arrange
            var id = 1;

            var sprint = new Sprint
            {
                Id = id,
                Name = "Old Sprint",
                Description = "Old Description",
                StartDate = new DateTime(2023, 7, 26),
                EndDate = new DateTime(2023, 8, 26)
            };

            var updateSprintCommand = new UpdateSprintCommand
            {
                Name = "New Sprint",
                Description = "NewDescription",
                StartDate = new DateTime(2023, 8, 16),
                EndDate = new DateTime(2023, 5, 13)
            };

            _dbContext.Sprints.Add(sprint);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _sut.UpdateAsync(id, updateSprintCommand, It.IsAny<CancellationToken>());

            // Assert
            Assert.Equal(updateSprintCommand.Name, result.Name);
            Assert.Equal(updateSprintCommand.Description, result.Description);
            Assert.Equal(updateSprintCommand.StartDate, result.StartDate);
            Assert.Equal(updateSprintCommand.EndDate, result.EndDate);
        }

        [Fact]
        public async Task Create_ShouldCreateAndReturnNewSprint()
        {
            // Arrange
            var createSprintCommand = new CreateSprintCommand
            {
                Name = "Test Sprint",
                Description = "Old Description",
                StartDate = new DateTime(2023, 7, 26),
                EndDate = new DateTime(2023, 8, 26)
            };

            var expectedSprint = _mapper.Map<Sprint>(createSprintCommand);
            expectedSprint.Id = 1;

            // Act
            var result = await _sut.CreateAsync(createSprintCommand, It.IsAny<CancellationToken>());

            // Assert
            Assert.Equal(createSprintCommand.Name, result.Name);
            Assert.Equal(createSprintCommand.Description, result.Description);
            Assert.Equal(createSprintCommand.StartDate, result.StartDate);
            Assert.Equal(createSprintCommand.EndDate, result.EndDate);
        }

        [Fact]
        public async Task Delete_ShouldSetIsDeletedPropertyToTrue()
        {
            // Arrange
            var sprintId = 1;
            var deletedSprint = new Sprint { Id = sprintId };
            _dbContext.Sprints.Add(deletedSprint);
            await _dbContext.SaveChangesAsync();

            _transactionUnitMock.Setup(t => t.ExecuteWithTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<TransactionScopeOption>(), It.IsAny<TransactionOptions>()))
                .Returns((Func<Task> operation, TransactionScopeOption option, TransactionOptions transactionOptions) => operation())
                .Verifiable();
            // Act
            await _sut.DeleteAsync(sprintId, It.IsAny<CancellationToken>());

            // Assert
            var sprint = _dbContext.Sprints.SingleOrDefault(s => s.Id == sprintId);
            Assert.NotNull(sprint);
            Assert.True(sprint.IsDeleted);
        }

        [Fact]
        public async Task CloseSprint_ShouldThrowNotFoundException_WhenSprintNotFound()
        {
            // Arrange
            var id = 1;

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                async () => await _sut.CloseSprint(id, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task CloseSprint_ShouldDeactivateSprint()
        {
            // Arrange
            var sprintId = 1;
            var sprint = new Sprint
            {
                Id = sprintId,
                Active = true
            };

            _dbContext.Sprints.Add(sprint);
            await _dbContext.SaveChangesAsync();

            // Act
            await _sut.CloseSprint(sprintId, It.IsAny<CancellationToken>());

            // Assert
            var updatedSprint = await _sut.GetAsync(sprintId, It.IsAny<CancellationToken>());
            Assert.False(updatedSprint.Active);
        }

        [Fact]
        public async Task PatchAsync_ShouldThrowNotFoundException_WhenSprintNotFound()
        {
            // Arrange
            var id = 1;
            var jsonPatchDocument = new JsonPatchDocument<SprintUpdatingDto>();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await _sut.PatchAsync(id, jsonPatchDocument, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task PatchAsync_ShouldThrowInvalidInputException_WhenInvalidPathInPatchDocument()
        {
            // Arrange
            var sprintId = 1;
            var sprint = new Sprint { Id = sprintId };

            _dbContext.Sprints.Add(sprint);
            await _dbContext.SaveChangesAsync();

            string jsonPatch = @"
            [
                {
                    ""op"": ""replace"",
                    ""path"": ""/CreatedAt"",
                    ""value"": ""2023-07-31T15:00:03.42""
                }
            ]";

            var patchDoc = JsonConvert.DeserializeObject<JsonPatchDocument<SprintUpdatingDto>>(jsonPatch);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidInputException>(() => _sut.PatchAsync(sprintId, patchDoc, default));
        }

        [Fact]
        public async Task PatchAsync_ShouldThrowsInvalidInputException_WhenInvalidValueInPatchDocument()
        {
            // Arrange
            var sprintId = 1;
            var sprint = new Sprint { Id = sprintId };

            _dbContext.Sprints.Add(sprint);
            await _dbContext.SaveChangesAsync();

            var patchDoc = new JsonPatchDocument<SprintUpdatingDto>();
            patchDoc.Replace(i => i.Name, null);

            var mockValidator = new Mock<IValidator<SprintUpdatingDto>>();
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be null") });
            mockValidator.Setup(v => v.ValidateAsync(It.IsAny<SprintUpdatingDto>(), default)).ReturnsAsync(validationResult);

            _validatorFactoryMock.Setup(f => f.GetValidator<SprintUpdatingDto>()).Returns(mockValidator.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidInputException>(() => _sut.PatchAsync(sprintId, patchDoc, default));
        }

        [Fact]
        public async Task PatchAsync_ShouldApplyPatchToSprintAndSaveChanges()
        {
            // Arrange
            var id = 1L;
            var sprint = new Sprint
            {
                Id = id,
                Name = "Old Name",
                Description = "Old Description",
                StartDate = new DateTime(2023, 7, 26),
                EndDate = new DateTime(2023, 8, 26)
            };

            _dbContext.Sprints.Add(sprint);
            await _dbContext.SaveChangesAsync();

            var patchedName = "Patched Name";
            var patchedDescription = "Patched Description";

            var jsonPatchDocument = new JsonPatchDocument<SprintUpdatingDto>();
            jsonPatchDocument.Replace(x => x.Name, patchedName);
            jsonPatchDocument.Replace(x => x.Description, patchedDescription);

            var mockValidator = new Mock<IValidator<SprintUpdatingDto>>();
            mockValidator.Setup(v => v.ValidateAsync(It.IsAny<SprintUpdatingDto>(), default)).ReturnsAsync(new ValidationResult());

            _validatorFactoryMock.Setup(f => f.GetValidator<SprintUpdatingDto>()).Returns(mockValidator.Object);
            // Act
            await _sut.PatchAsync(id, jsonPatchDocument, CancellationToken.None);
            var updatedSprint = await _dbContext.Sprints.FindAsync(id);

            // Assert
            Assert.Equal(sprint.Id, updatedSprint.Id);
            Assert.Equal(patchedName, updatedSprint.Name);
            Assert.Equal(patchedDescription, updatedSprint.Description);
            Assert.Equal(sprint.StartDate, updatedSprint.StartDate);
            Assert.Equal(sprint.EndDate, updatedSprint.EndDate);
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
            var sprint = new Sprint { Name = "TestSprint" };
            _dbContext.Sprints.Add(sprint);
            await _dbContext.SaveChangesAsync();

            // Act
            var results = await _sut.SearchAsync("Name", "NonMatchingValue", 50, CancellationToken.None);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnSprintsList_WhenMatching()
        {
            // Arrange
            var sprint1 = new Sprint { Name = "SameName" };
            var sprint2 = new Sprint { Name = "SameName" };
            var sprint3 = new Sprint { Name = "DifferentName" };
            _dbContext.Sprints.Add(sprint1);
            _dbContext.Sprints.Add(sprint2);
            _dbContext.Sprints.Add(sprint3);
            await _dbContext.SaveChangesAsync();

            var sprintsList = new List<Sprint> { sprint1, sprint2 };

            // Act
            var results = await _sut.SearchAsync("Name", "SameName", 50, CancellationToken.None);

            // Assert
            Assert.Equal(sprintsList, results);
        }
    }
}
