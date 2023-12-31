﻿namespace IssueTracker.Abstractions.Models
{
    public class Sprint : IEntityWithId, ICreationTracking, IModificationTracking, ISoftDeletable
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
