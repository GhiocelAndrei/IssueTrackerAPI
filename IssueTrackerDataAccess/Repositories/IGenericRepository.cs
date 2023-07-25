using System.Linq.Expressions;

namespace IssueTracker.DataAccess.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        ValueTask<T> GetAsync(long id, CancellationToken cancellationToken);
        Task<T> GetUniqueWithConditionAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
        Task<List<T>> GetAllAsync(CancellationToken cancellationToken);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken);
        Task<T> UpdateAsync(T entity, CancellationToken cancellationToken);
        Task<T> DeleteAsync(long id, CancellationToken cancellationToken);
        Task<bool> ExistsWithConditionAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
    }
}
