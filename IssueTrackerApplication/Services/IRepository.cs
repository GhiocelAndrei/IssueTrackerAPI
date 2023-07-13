
namespace IssueTracker.Application.Services
{
    public interface IRepository<T> where T : class
    {
        Task<T> Get(long id);
        Task<IEnumerable<T>> GetAll();
        Task<(bool isSuccess, string message, T entity)> Add(T entity);
        Task<T> Update(T entity);
        Task<T> Delete(long id);
        Task<bool> Exists(long id);
    }
}
