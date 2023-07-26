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
    public class UserServiceTests : IDisposable
    {
        private IssueContext _dbContext;
        private readonly IMapper _mapper;
        private readonly UserService _sut;

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

            _sut = new UserService(_dbContext, _mapper);
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
            _dbContext.SaveChanges();

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
            _dbContext.SaveChanges();

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
            _dbContext.SaveChanges();

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
        public async Task Delete_ShouldDeleteAndReturnDeletedUser()
        {
            // Arrange
            var userId = 1;
            var deletedUser = new User { Id = userId };
            _dbContext.Users.Add(deletedUser);
            await _dbContext.SaveChangesAsync();

            // Act
            await _sut.DeleteAsync(userId, It.IsAny<CancellationToken>());

            // Assert
            Assert.False(_dbContext.Users.Any(user => user.Id == userId));
        }

        [Fact]
        public async Task LoginUserAsync_ShouldReturnUserRole()
        {
            // Arrange
            var loginUserCommand = new LoginUserCommand 
            { 
                Name = "John", 
                Email = "john@test.com" 
            };

            var user = new User 
            { 
              Name = "John", 
              Email = "john@test.com",
              Role = "Admin" 
            };

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            // Act
            var result = await _sut.LoginUserAsync(loginUserCommand, It.IsAny<CancellationToken>());

            // Assert
            Assert.Equal(user.Role, result);
        }
    }
}
