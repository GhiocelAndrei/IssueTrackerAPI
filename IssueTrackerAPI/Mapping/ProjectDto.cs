using IssueTrackerAPI.Models;

namespace IssueTrackerAPI.Mapping
{
    public class ProjectDto
    {
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class ProjectCreatingDto
    {
        public string Name { get; set; }
    }
}
