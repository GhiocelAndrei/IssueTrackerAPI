namespace IssueTracker.Abstractions.Models
{
    public class Project : ICreationTracking, IModificationTracking, ISoftDeletable
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
