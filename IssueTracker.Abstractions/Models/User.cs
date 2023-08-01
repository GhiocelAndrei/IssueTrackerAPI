namespace IssueTracker.Abstractions.Models
{
    public class User : IEntityWithId, ISoftDeletable
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
