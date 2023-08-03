using System.Transactions;

namespace IssueTracker.Application.Services
{
    public interface IUnitOfWork
    {
        Task ExecuteWithTransactionAsync(Func<Task> operation, TransactionScopeOption scopeOption, TransactionOptions transaction);

        Task<T> ExecuteWithTransactionAsync<T>(Func<Task<T>> operation, TransactionScopeOption scopeOption, TransactionOptions transaction);
    }
}
