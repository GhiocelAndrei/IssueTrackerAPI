namespace IssueTracker.Abstractions.Models
{
    public interface ICreationTracking
    {
        DateTime CreatedAt { get; set; }
    }

    public interface IModificationTracking
    {
        DateTime? UpdatedAt { get; set; }
    }

    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
    }

    public interface IEntityWithId
    {
        long Id { get; set; }
    }
}
