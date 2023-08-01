namespace IssueTracker.Abstractions.Mapping
{
    public class SprintDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class SprintCreatingDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class CreateSprintCommand
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class SprintUpdatingDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? Active { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class UpdateSprintCommand
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? Active { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class SprintCreatingWithIssuesDto
    {
        public SprintCreatingDto SprintDto { get; set; }
        public List<long> Ids { get; set; }
    }

    public class CreateSprintWithIssuesCommand
    {
        public CreateSprintCommand SprintDto { get; set; }
        public List<long> Ids { get; set; }
    }
}
