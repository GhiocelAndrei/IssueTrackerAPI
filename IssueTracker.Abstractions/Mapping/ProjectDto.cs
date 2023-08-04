namespace IssueTracker.Abstractions.Mapping
{
    public class ProjectDto
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProjectCreatingDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class CreateProjectCommand
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public long IssueSequence { get; set; } = 0;
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
