namespace IssueTracker.Abstractions.Mapping
{
    public class UserIssueDto
    {
        public string ExternalId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
