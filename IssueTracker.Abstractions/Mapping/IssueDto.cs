using IssueTracker.Abstractions.Enums;

namespace IssueTracker.Abstractions.Mapping
{
    public class IssueDto
    {
        public long Id { get; set; }
        public string ExternalId { get; set; }
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

    public class CreateIssueCommand
    {
        public string ExternalId { get; set; }
        public long ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public long ReporterId { get; set; }
        public long AssigneeId { get; set; }
    }

    public class IssueUpdatingDto
    {
        public long? ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Priority? Priority { get; set; }
        public long? ReporterId { get; set; }
        public long? AssigneeId { get; set; }
    }

    public class UpdateIssueCommand
    {
        public long? ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Priority? Priority { get; set; }
        public long? ReporterId { get; set; }
        public long? AssigneeId { get; set; }
    }
}
