﻿namespace IssueTracker.Abstractions.Models
{
    public class User : IHasId
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
