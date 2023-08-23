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
        private readonly SearchLimitingService _searchLimitingService;

        public SearchController(IIssuesService issuesService, IProjectsService projectService, SearchLimitingService searchLimitingService)
        {
            _issuesService = issuesService;
            _projectsService = projectService;
            _searchLimitingService = searchLimitingService;
        }


        [HttpGet]
        [OAuth(Scopes.IssuesRead, Scopes.ProjectsRead)]
        public async Task<IActionResult> SearchForKeyword(string keyword, CancellationToken ct)
        {
            if (keyword.Length < 3)
            {
                return BadRequest("Keyword minimum length : 3 characters");
            }

            var issuesResults = await _issuesService.SearchAsync(new List<string> { "Title", "Description" }, keyword, SearchLimitingService.MaxResultsPerCategory * 2, ct);
            var projectsResults = await _projectsService.SearchAsync("Name", keyword, SearchLimitingService.MaxResultsPerCategory * 2, ct);

            var (limitedIssues, limitedProjects) = _searchLimitingService.LimitResults(issuesResults, projectsResults);

            var combinedResults = new
            {
                Projects = limitedProjects,
                Issues = limitedIssues
            };

            return Ok(combinedResults);
        }
    }
}
