using System.Linq.Expressions;

namespace IssueTracker.DataAccess.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        ValueTask<T> GetAsync(long id);
        Task<T> GetUniqueWithConditionAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(long id);
        Task<bool> ExistsAsync(long id);
        Task<bool> ExistsWithConditionAsync(Expression<Func<T, bool>> predicate);
    }
}
