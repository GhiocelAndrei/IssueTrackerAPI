using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.Services
{
    public interface IBaseService<T, TCreateCommand, TUpdateCommand>
        where T : class
        where TCreateCommand : class
        where TUpdateCommand : class
    {
        Task<List<T>> GetAllAsync(CancellationToken ct);
        Task<T> GetAsync(long id, CancellationToken ct);
        Task<T> UpdateAsync(long id, TUpdateCommand command, CancellationToken ct);
        Task<T> CreateAsync(TCreateCommand entity, CancellationToken ct);
        Task DeleteAsync(long id, CancellationToken ct);
    }
}
