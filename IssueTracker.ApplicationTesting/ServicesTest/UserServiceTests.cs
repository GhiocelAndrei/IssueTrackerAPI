using Moq;
using AutoMapper;
using IssueTracker.Application.Services;
using IssueTracker.DataAccess.Repositories;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Exceptions;

namespace IssueTracker.Testing.ServicesTest
{
    public class UserServiceTests
    {
        private readonly Mock<IGenericRepository<User>> _mockRepository;
        private readonly IMapper _mapper;
        private readonly UserService _sut;

        public UserServiceTests()
        {
            _mockRepository = new Mock<IGenericRepository<User>>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            _mapper = mapperConfig.CreateMapper();

            _sut = new UserService(_mockRepository.Object, _mapper);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User> { new User(), new User() };
            _mockRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

            // Act
            var result = await _sut.GetAllAsync(It.IsAny<CancellationToken>());

            // Assert
            _mockRepository.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task Get_ById_ShouldThrowNotFoundException_WhenIssueNotFound()
        {
            // Arrange
            var id = 1;
            _mockRepository.Setup(r => r.GetAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await _sut.GetAsync(id, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Get_ById_ShouldReturnUser()
        {
            // Arrange
            var userId = 3;

            var returnedUser = new User
            {
                Id = userId
            };

            _mockRepository.Setup(x => x.GetAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(returnedUser);

            // Act
            var user = await _sut.GetAsync(userId, It.IsAny<CancellationToken>());

            // Assert
            _mockRepository.Verify(x => x.GetAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(userId, user.Id);
        }

        [Fact]
        public async Task Update_ShouldThrowNotFoundException_WhenIssueNotFound()
        {
            // Arrange
            var id = 1;
            var command = new UpdateUserCommand();
            _mockRepository.Setup(r => r.GetAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await _sut.UpdateAsync(id, command, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Update_ShouldUpdateAndReturnUpdatedUser()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
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

            _mockRepository.Setup(r => r.GetAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((User u, CancellationToken cancellationToken) => u);

            // Act
            var result = await _sut.UpdateAsync(1, updateUserCommand, It.IsAny<CancellationToken>());

            // Assert
            _mockRepository.Verify(r => r.GetAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);

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

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedUser);

            // Act
            var result = await _sut.CreateAsync(createUserCommand, It.IsAny<CancellationToken>());

            // Assert
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);

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
            var deletedUser = new User();

            _mockRepository.Setup(r => r.GetAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(deletedUser);
            _mockRepository.Setup(r => r.DeleteAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(deletedUser);

            // Act
            await _sut.DeleteAsync(userId, It.IsAny<CancellationToken>());

            // Assert
            _mockRepository.Verify(r => r.DeleteAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
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

            _mockRepository.Setup(repo => repo.GetUniqueWithConditionAsync(u => u.Name == loginUserCommand.Name && u.Email == loginUserCommand.Email, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);

            // Act
            var result = await _sut.LoginUserAsync(loginUserCommand, It.IsAny<CancellationToken>());

            // Assert
            _mockRepository.Verify(repo =>
                repo.GetUniqueWithConditionAsync(u => u.Name == loginUserCommand.Name && u.Email == loginUserCommand.Email, It.IsAny<CancellationToken>()), Times.Once);
            
            Assert.Equal(user.Role, result);
        }
    }
}
