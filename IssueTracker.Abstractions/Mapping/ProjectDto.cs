namespace IssueTracker.Abstractions.Mapping
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

    public class CreateProjectCommand
    {
        public string Name { get; set; }
    }

    public class ProjectUpdatingDto
    {
        public string Name { get; set; }
    }

    public class UpdateProjectCommand
    {
        public string Name { get; set; }
    }
}
