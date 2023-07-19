using System.Linq.Expressions;

namespace IssueTracker.DataAccess.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetAsync(long id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(long id);
        Task<bool> ExistsAsync(long id);
        Task<bool> ExistsWithConditionAsync(Expression<Func<T, bool>> predicate);
    }
}
