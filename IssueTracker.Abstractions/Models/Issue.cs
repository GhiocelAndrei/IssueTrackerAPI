using IssueTracker.Abstractions.Enums;

namespace IssueTracker.Abstractions.Models
{
    public class Issue : IEntityWithId, ICreationTracking, IModificationTracking, ISoftDeletable
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public long ReporterId { get; set; }
        public User Reporter { get; set; }
        public long AssigneeId { get; set; }
        public User Assignee { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
