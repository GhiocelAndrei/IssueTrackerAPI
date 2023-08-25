using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;
using IssueTracker.Application.Services;
using IssueTracker.DataAccess.Repositories;
using Moq;

namespace IssueTracker.ApplicationTesting.ServicesTest
{
    public class SearchServiceTest
    {
        private readonly Mock<ISearchRepository> searchRepositoryMock;
        private readonly SearchService _sut;

        public SearchServiceTest()
        {
            searchRepositoryMock = new Mock<ISearchRepository>();

            _sut = new SearchService(searchRepositoryMock.Object);
        }

        [Fact]
        public void LimitResults_ShouldReturnLimitedResults()
        {
            var issuesResults = Enumerable.Range(1, 120).Select(x => new Issue { Id = x }).ToList();
            var searchsResults = Enumerable.Range(1, 60).Select(x => new Project { Id = x }).ToList();

            var (limitedIssues, limitedSearchs) = _sut.LimitResults(issuesResults, searchsResults);

            Assert.Equal(SearchService.MaxResultsPerCategory, limitedIssues.Count);
            Assert.Equal(SearchService.MaxResultsPerCategory, limitedSearchs.Count);
        }

        [Fact]
        public void LimitResults_ShouldReturnResultsEqualToLimit()
        {
            var issuesResults = Enumerable.Range(1, 30).Select(x => new Issue { Id = x }).ToList();
            var searchsResults = Enumerable.Range(1, 70).Select(x => new Project { Id = x }).ToList();

            var (limitedIssues, limitedSearchs) = _sut.LimitResults(issuesResults, searchsResults);

            Assert.Equal(30, limitedIssues.Count);
            Assert.Equal(70, limitedSearchs.Count);
        }
    }
}
