using Moq;
using AutoMapper;
using IssueTracker.Application.Services;
using IssueTracker.DataAccess.Repositories;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Enums;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Exceptions;

namespace IssueTracker.Testing.ServicesTest
{
    public class IssueServiceTests
    {
        private readonly Mock<IGenericRepository<Issue>> _mockRepository;
        private readonly IMapper _mapper;
        private readonly IssueService _sut;

        public IssueServiceTests()
        {
            _mockRepository = new Mock<IGenericRepository<Issue>>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            _mapper = mapperConfig.CreateMapper();

            _sut = new IssueService(_mockRepository.Object, _mapper);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllIssues()
        {
            // Arrange
            var issues = new List<Issue> { new Issue(), new Issue() };
            _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(issues);

            // Act
            var result = await _sut.GetAll();

            // Assert
            _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task Get_ById_ShouldThrowNotFoundException_WhenIssueNotFound()
        {
            // Arrange
            var id = 1;
            _mockRepository.Setup(r => r.GetAsync(id)).ReturnsAsync((Issue)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await _sut.Get(id));
        }


        [Fact]
        public async Task Get_ById_ShouldReturnIssue()
        {
            // Arrange
            var issuesId = 3;

            var returnedIssue = new Issue
            {
                Id = issuesId
            };

            _mockRepository.Setup(x => x.GetAsync(issuesId)).ReturnsAsync(returnedIssue);

            // Act
            var issue = await _sut.Get(issuesId);

            // Assert
            _mockRepository.Verify(x => x.GetAsync(issuesId), Times.Once);
            Assert.Equal(issuesId, issue.Id);
        }

        [Fact]
        public async Task Update_ShouldThrowNotFoundException_WhenIssueNotFound()
        {
            // Arrange
            var id = 1;
            var command = new UpdateIssueCommand();
            _mockRepository.Setup(r => r.GetAsync(id)).ReturnsAsync((Issue)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await _sut.Update(id, command));
        }


        [Fact]
        public async Task Update_ShouldUpdateAndReturnUpdatedIssue()
        {
            // Arrange
            var issue = new Issue
            {
                Id = 1,
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

            _mockRepository.Setup(r => r.GetAsync(1)).ReturnsAsync(issue);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Issue>())).ReturnsAsync((Issue i) => i);

            // Act
            var result = await _sut.Update(1, updateIssueCommand);

            // Assert
            _mockRepository.Verify(r => r.GetAsync(1), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(issue), Times.Once);

            Assert.Equal(issue, result);
            Assert.Equal(updateIssueCommand.Title, result.Title);
            Assert.Equal(updateIssueCommand.Description, result.Description);
            Assert.Equal(updateIssueCommand.Priority, result.Priority);
            Assert.Equal(updateIssueCommand.ReporterId, result.ReporterId);
            Assert.Equal(updateIssueCommand.AssigneeId, result.AssigneeId);
            Assert.Equal(updateIssueCommand.ProjectId, result.ProjectId);
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

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Issue>())).ReturnsAsync(expectedIssue);

            // Act
            var result = await _sut.Create(createIssueCommand);

            // Assert
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Issue>()), Times.Once);

            Assert.Equal(expectedIssue.Id, result.Id);
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
            var deletedIssue = new Issue();

            _mockRepository.Setup(r => r.GetAsync(issueId)).ReturnsAsync(deletedIssue);
            _mockRepository.Setup(r => r.DeleteAsync(issueId)).ReturnsAsync(deletedIssue);

            // Act
            await _sut.Delete(issueId);

            // Assert
            _mockRepository.Verify(r => r.DeleteAsync(issueId), Times.Once);
        }
    }
}
