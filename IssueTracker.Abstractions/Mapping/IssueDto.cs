using IssueTracker.Abstractions.Enums;

namespace IssueTracker.Abstractions.Mapping
{
    public class IssueDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class IssueCreatingDto
    {
        public long ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public long ReporterId { get; set; }
        public long AssigneeId { get; set; }
    }

    public class IssueCommandDto
    {
        public long ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public long ReporterId { get; set; }
        public long AssigneeId { get; set; }
    }
}
