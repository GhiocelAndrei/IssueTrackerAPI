using Moq;
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

namespace IssueTracker.Testing.ServicesTest
{
    public class UserServiceTests : IDisposable
    {
        private IssueContext _dbContext;
        private readonly IMapper _mapper;
        private readonly UsersService _sut;
        private readonly Mock<IValidatorFactory> _validatorFactoryMock;

        public UserServiceTests()
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

            _sut = new UsersService(_dbContext, _mapper, _validatorFactoryMock.Object);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            _dbContext = null;
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllUsers()
        {
            // Arrange
            _dbContext.Users.Add(new User());
            _dbContext.Users.Add(new User());
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _sut.GetAllAsync(It.IsAny<CancellationToken>());

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task Get_ById_ShouldThrowNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var id = 1;

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await _sut.GetAsync(id, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Get_ById_ShouldReturnUser()
        {
            // Arrange
            var userId = 1;
            var user = new User { Id = userId };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var returnedUser = await _sut.GetAsync(userId, It.IsAny<CancellationToken>());

            // Assert
            Assert.Equal(userId, returnedUser.Id);
        }

        [Fact]
        public async Task Update_ShouldThrowNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var id = 1;
            var command = new UpdateUserCommand();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await _sut.UpdateAsync(id, command, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Update_ShouldUpdateAndReturnUpdatedUser()
        {
            // Arrange
            var id = 1;

            var user = new User
            {
                Id = id,
                Name = "User",
                Email = "user@yahoo.com",
                Role = "User"
            };
            var updateUserCommand = new UpdateUserCommand
            {
                Name = "New User",
                Email = "newuser@yahoo.com",
                Role = "Admin"
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _sut.UpdateAsync(id, updateUserCommand, It.IsAny<CancellationToken>());

            // Assert
            Assert.Equal(user, result);
            Assert.Equal(updateUserCommand.Name, result.Name);
            Assert.Equal(updateUserCommand.Email, result.Email);
            Assert.Equal(updateUserCommand.Role, result.Role);
        }

        [Fact]
        public async Task Create_ShouldCreateAndReturnNewUser()
        {
            // Arrange
            var createUserCommand = new CreateUserCommand
            {
                Name = "New User",
                Email = "newuser@yahoo.com",
                Role = "User"
            };

            var expectedUser = _mapper.Map<User>(createUserCommand);
            expectedUser.Id = 1;

            // Act
            var result = await _sut.CreateAsync(createUserCommand, It.IsAny<CancellationToken>());

            // Assert
            Assert.Equal(expectedUser.Id, result.Id);
            Assert.Equal(expectedUser.Name, result.Name);
            Assert.Equal(expectedUser.Email, result.Email);
            Assert.Equal(expectedUser.Role, result.Role);
        }

        [Fact]
        public async Task Delete_ShouldSetIsDeletedPropertyToTrue()
        {
            // Arrange
            var userId = 1;
            var deletedUser = new User { Id = userId };
            _dbContext.Users.Add(deletedUser);
            await _dbContext.SaveChangesAsync();

            // Act
            await _sut.DeleteAsync(userId, It.IsAny<CancellationToken>());

            // Assert
            var user = _dbContext.Users.SingleOrDefault(u => u.Id == userId);
            Assert.NotNull(user);
            Assert.True(user.IsDeleted);
        }

        [Fact]
        public async Task PatchAsync_ShouldThrowNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var id = 1;
            var jsonPatchDocument = new JsonPatchDocument<UserUpdatingDto>();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await _sut.PatchAsync(id, jsonPatchDocument, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task PatchAsync_ShouldThrowInvalidInputException_WhenInvalidPathInPatchDocument()
        {
            // Arrange
            var userId = 1;
            var user = new User { Id = userId };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            string jsonPatch = @"
            [
                {
                    ""op"": ""replace"",
                    ""path"": ""/DeletedAt"",
                    ""value"": ""2023-07-31T15:00:03.42""
                }
            ]";

            var patchDoc = JsonConvert.DeserializeObject<JsonPatchDocument<UserUpdatingDto>>(jsonPatch);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidInputException>(() => _sut.PatchAsync(userId, patchDoc, default));
        }

        [Fact]
        public async Task PatchAsync_ShouldThrowsInvalidInputException_WhenInvalidValueInPatchDocument()
        {
            // Arrange
            var userId = 1;
            var user = new User { Id = userId };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var patchDoc = new JsonPatchDocument<UserUpdatingDto>();
            patchDoc.Replace(i => i.Name, null);

            var mockValidator = new Mock<IValidator<UserUpdatingDto>>();
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be null") });
            mockValidator.Setup(v => v.ValidateAsync(It.IsAny<UserUpdatingDto>(), default)).ReturnsAsync(validationResult);

            _validatorFactoryMock.Setup(f => f.GetValidator<UserUpdatingDto>()).Returns(mockValidator.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidInputException>(() => _sut.PatchAsync(userId, patchDoc, default));
        }

        [Fact]
        public async Task PatchAsync_ShouldApplyPatchToUserAndSaveChanges()
        {
            // Arrange
            var id = 1L;
            var user = new User
            {
                Id = id,
                Name = "New User",
                Email = "newuser@yahoo.com",
                Role = "User"
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var patchedName = "Patched Name";
            var patchedRole = "Admin";

            var jsonPatchDocument = new JsonPatchDocument<UserUpdatingDto>();
            jsonPatchDocument.Replace(x => x.Name, patchedName);
            jsonPatchDocument.Replace(x => x.Role, patchedRole);

            var mockValidator = new Mock<IValidator<UserUpdatingDto>>();
            mockValidator.Setup(v => v.ValidateAsync(It.IsAny<UserUpdatingDto>(), default)).ReturnsAsync(new ValidationResult());

            _validatorFactoryMock.Setup(f => f.GetValidator<UserUpdatingDto>()).Returns(mockValidator.Object);
            // Act
            await _sut.PatchAsync(id, jsonPatchDocument, CancellationToken.None);
            var updatedUser = await _dbContext.Users.FindAsync(id);

            // Assert
            Assert.Equal(user.Id, updatedUser.Id);
            Assert.Equal(patchedName, updatedUser.Name);
            Assert.Equal(patchedRole, updatedUser.Role);
            Assert.Equal(user.Email, updatedUser.Email);
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
            var user = new User { Name = "TestUser" };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var results = await _sut.SearchAsync("Name", "NonMatchingValue", 50, CancellationToken.None);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnUsersList_WhenMatching()
        {
            // Arrange
            var user1 = new User { Name = "SameName" };
            var user2 = new User { Name = "SameName" };
            var user3 = new User { Name = "DifferentName" };
            _dbContext.Users.Add(user1);
            _dbContext.Users.Add(user2);
            _dbContext.Users.Add(user3);
            await _dbContext.SaveChangesAsync();

            var usersList = new List<User> { user1, user2 };

            // Act
            var results = await _sut.SearchAsync("Name", "SameName", 50, CancellationToken.None);

            // Assert
            Assert.Equal(usersList, results);
        }
    }
}
