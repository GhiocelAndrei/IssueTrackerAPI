using IssueTracker.Abstractions.Models;
using IssueTracker.Application.Services;

namespace IssueTracker.ApplicationTesting.ServicesTest
{
    public class SearchLimitingServiceTest
    {
        private readonly SearchLimitingService _sut;

        public SearchLimitingServiceTest()
        {
            _sut = new SearchLimitingService();
        }

        [Fact]
        public void LimitResults_ShouldReturnLimitedResults()
        {
            var issuesResults = Enumerable.Range(1, 120).Select(x => new Issue { Id = x }).ToList();
            var projectsResults = Enumerable.Range(1, 60).Select(x => new Project { Id = x }).ToList();

            var (limitedIssues, limitedProjects) = _sut.LimitResults(issuesResults, projectsResults);

            Assert.Equal(SearchLimitingService.MaxResultsPerCategory, limitedIssues.Count);
            Assert.Equal(SearchLimitingService.MaxResultsPerCategory, limitedProjects.Count);
        }

        [Fact]
        public void LimitResults_ShouldReturnResultsEqualToLimit()
        {
            var issuesResults = Enumerable.Range(1, 30).Select(x => new Issue { Id = x }).ToList();
            var projectsResults = Enumerable.Range(1, 70).Select(x => new Project { Id = x }).ToList();

            var (limitedIssues, limitedProjects) = _sut.LimitResults(issuesResults, projectsResults);

            Assert.Equal(30, limitedIssues.Count);
            Assert.Equal(70, limitedProjects.Count);
        }
    }
}
