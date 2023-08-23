﻿namespace IssueTracker.Application.Services
{
    public class SearchLimitingService
    {
        public const int MaxResultsPerCategory = 50;

        public (List<T1> issues, List<T2> projects) LimitResults<T1, T2>(List<T1> issuesResults, List<T2> projectsResults)
        {
            var availableIssues = issuesResults.Count;
            var availableProjects = projectsResults.Count;

            var issuesToTake = Math.Min(MaxResultsPerCategory, availableIssues);
            var projectsToTake = Math.Min(MaxResultsPerCategory, availableProjects);

            var extraIssuesSlots = MaxResultsPerCategory - issuesToTake;
            var extraProjectsSlots = MaxResultsPerCategory - projectsToTake;

            issuesToTake += Math.Min(extraProjectsSlots, availableIssues - issuesToTake);
            projectsToTake += Math.Min(extraIssuesSlots, availableProjects - projectsToTake);

            issuesResults = issuesResults.GetRange(0, issuesToTake);
            projectsResults = projectsResults.GetRange(0, projectsToTake);

            return (issuesResults, projectsResults);
        }
    }
}
