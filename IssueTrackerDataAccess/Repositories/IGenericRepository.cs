using System.Linq.Expressions;

namespace IssueTracker.DataAccess.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> Get(long id);
        Task<IEnumerable<T>> GetAll();
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task<T> Delete(long id);
        Task<bool> Exists(long id);
        Task<bool> ExistsWithCondition(Expression<Func<T, bool>> predicate);
    }
}
