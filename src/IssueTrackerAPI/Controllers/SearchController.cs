using IssueTracker.Abstractions.Definitions;
using IssueTracker.Application.Authorization;
using IssueTracker.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace IssueTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IIssuesService _issuesService;
        private readonly IProjectsService _projectsService;
        private readonly SearchService _searchService;

        public SearchController(IIssuesService issuesService, IProjectsService projectService, SearchService searchService)
        {
            _issuesService = issuesService;
            _projectsService = projectService;
            _searchService = searchService;
        }

        /*
         * This method isn't optimal since we are doing two different queries :
         */
        [HttpGet]
        [OAuth(Permissions.IssuesRead, Permissions.ProjectsRead)]
        public async Task<IActionResult> SearchForKeyword(string keyword, CancellationToken ct)
        {
            if (keyword.Length < 3)
            {
                return BadRequest("Keyword minimum length : 3 characters");
            }

            var issuesResults = await _issuesService.SearchAsync(new List<string> { "Title", "Description" }, keyword, SearchService.MaxResultsPerCategory * 2, ct);
            var projectsResults = await _projectsService.SearchAsync("Name", keyword, SearchService.MaxResultsPerCategory * 2, ct);

            var (limitedIssues, limitedProjects) = _searchService.LimitResults(issuesResults, projectsResults);

            var combinedResults = new
            {
                Projects = limitedProjects,
                Issues = limitedIssues
            };

            return Ok(combinedResults);
        }

        [HttpGet("Optimized")]
        [OAuth(Permissions.IssuesRead, Permissions.ProjectsRead)]
        public async Task<IActionResult> SearchForKeywordOptimized(string keyword, CancellationToken ct)
        {
            if (keyword.Length < 3)
            {
                return BadRequest("Keyword minimum length : 3 characters");
            }

            var combinedResults = await _searchService.SearchAsync(keyword, ct);

            return Ok(combinedResults);
        }
    }
}
