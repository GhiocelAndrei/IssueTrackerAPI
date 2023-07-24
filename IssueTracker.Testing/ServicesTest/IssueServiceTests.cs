using Moq;
using AutoMapper;
using IssueTracker.Application.Services;
using IssueTracker.DataAccess.Repositories;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Enums;
using IssueTracker.Abstractions.Mapping;

namespace IssueTracker.Testing.ServicesTest
{
    public class IssueServiceTests
    {
        private readonly Mock<IGenericRepository<Issue>> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly IssueService _issueService;

        public IssueServiceTests()
        {
            _mockRepository = new Mock<IGenericRepository<Issue>>();
            _mockMapper = new Mock<IMapper>();
            _issueService = new IssueService(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllIssues()
        {
            // Arrange
            var issues = new List<Issue> { new Issue(), new Issue() };
            _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(issues);

            // Act
            var result = await _issueService.GetAll();

            // Assert
            _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
            Assert.Equal(2, result.Count());
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
            var issue = await _issueService.Get(issuesId);

            // Assert
            Assert.Equal(issuesId, issue.Id);
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
            _mockMapper.Setup(m => m.Map(updateIssueCommand, issue)).Callback((UpdateIssueCommand source, Issue target) => {
                target.Title = source.Title;
                target.Description = source.Description;
                target.Priority = source.Priority;
                target.ReporterId = source.ReporterId;
                target.AssigneeId = source.AssigneeId;
                target.ProjectId = source.ProjectId;
            });

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Issue>())).ReturnsAsync((Issue i) => i);

            // Act
            var result = await _issueService.Update(1, updateIssueCommand);

            // Assert
            _mockRepository.Verify(r => r.GetAsync(1), Times.Once);
            _mockMapper.Verify(m => m.Map(updateIssueCommand, issue), Times.Once);
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

            var createdIssue = new Issue
            {
                Id = 1,
                Title = createIssueCommand.Title,
                Description = createIssueCommand.Description,
                Priority = createIssueCommand.Priority,
                ReporterId = createIssueCommand.ReporterId,
                AssigneeId = createIssueCommand.AssigneeId,
                ProjectId = createIssueCommand.ProjectId
            };

            _mockMapper.Setup(m => m.Map<Issue>(createIssueCommand)).Returns(createdIssue);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Issue>())).ReturnsAsync((Issue i) => i);

            // Act
            var result = await _issueService.Create(createIssueCommand);

            // Assert
            _mockMapper.Verify(m => m.Map<Issue>(createIssueCommand), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(createdIssue), Times.Once);

            Assert.Equal(createdIssue, result);
        }

        [Fact]
        public async Task Delete_ShouldDeleteAndReturnDeletedIssue()
        {
            // Arrange
            var issueId = 1;
            var deletedIssue = new Issue(); // Initialize with test data if necessary
            _mockRepository.Setup(r => r.GetAsync(issueId)).ReturnsAsync(deletedIssue);
            _mockRepository.Setup(r => r.DeleteAsync(issueId)).ReturnsAsync(deletedIssue);

            // Act
            await _issueService.Delete(issueId);

            // Assert
            _mockRepository.Verify(r => r.DeleteAsync(issueId), Times.Once);
        }


    }
}
