﻿using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;

namespace IssueTracker.Application.Services
{
    public interface IProjectsService : IBaseService<Project>
    {
        Task<bool> ExistsAsync(long id, CancellationToken ct);
        Task<long> GetIssueSequenceAsync(long projectId, CancellationToken ct);
        Task<string> GetProjectCodeAsync(long projectId, CancellationToken ct);
    }
}
