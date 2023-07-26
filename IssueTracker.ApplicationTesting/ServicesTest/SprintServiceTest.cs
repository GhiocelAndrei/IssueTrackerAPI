using Moq;
using AutoMapper;
using IssueTracker.Application.Services;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Exceptions;
using IssueTracker.DataAccess.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.ApplicationTesting.ServicesTest
{
    public class SprintServiceTests : IDisposable
    {
        private IssueContext _dbContext;
        private readonly IMapper _mapper;
        private readonly SprintsService _sut;
        private readonly Mock<IIssuesService> _issuesServiceMock;

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

            _sut = new SprintsService(_dbContext, _mapper, _issuesServiceMock.Object);
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
        public async Task Delete_ShouldCallDeleteAsyncOnRepository()
        {
            // Arrange
            var sprintId = 1;
            var deletedSprint = new Sprint { Id = sprintId };
            _dbContext.Sprints.Add(deletedSprint);
            await _dbContext.SaveChangesAsync();

            // Act
            await _sut.DeleteAsync(sprintId, It.IsAny<CancellationToken>());

            // Assert
            Assert.False(_dbContext.Sprints.Any(sprint => sprint.Id == sprintId));
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
    }
}
