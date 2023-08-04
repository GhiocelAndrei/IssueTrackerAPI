namespace IssueTracker.Abstractions.Models
{
    public class Project : IEntityWithId, ICreationTracking, IModificationTracking, ISoftDeletable
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public long IssueSequence { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
