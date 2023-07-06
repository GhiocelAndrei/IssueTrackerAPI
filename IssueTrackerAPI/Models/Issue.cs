namespace IssueTrackerAPI.Models
{
    public class Issue : ICreationTracking, IModificationTracking, ISoftDeletable
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public User Reporter { get; set; }
        public User Asignee { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
    public enum Priority
    {
        Low, Medium, High
    }

}
