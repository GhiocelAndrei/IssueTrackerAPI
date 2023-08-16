using Microsoft.AspNetCore.JsonPatch;

namespace IssueTracker.Application.Services
{
    public interface IBaseService<T>
        where T : class
    {
        Task<List<T>> GetAllAsync(CancellationToken ct);
        Task<T> GetAsync(long id, CancellationToken ct);
        Task<T> UpdateAsync<TUpdateCommand>(long id, TUpdateCommand command, CancellationToken ct)
            where TUpdateCommand : class;
        Task<T> PatchAsync<TUpdateDTO>(long id, JsonPatchDocument<TUpdateDTO> patchDTO, CancellationToken ct) 
            where TUpdateDTO : class;
        Task<T> CreateAsync<TCreateCommand>(TCreateCommand entity, CancellationToken ct)
            where TCreateCommand : class;
        Task DeleteAsync(long id, CancellationToken ct);
        Task<List<T>> SearchAsync(string property, string value, int queryLimit, CancellationToken ct);
        Task<List<T>> SearchAsync(List<string> properties, string value, int queryLimit, CancellationToken ct);
    }
}
