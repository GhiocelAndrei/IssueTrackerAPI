using System.Transactions;

namespace IssueTracker.Application.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        public async Task ExecuteWithTransactionAsync(Func<Task> operation, TransactionScopeOption scopeOption, TransactionOptions transactionOptions)
        {
            using var transactionScope = new TransactionScope(scopeOption, transactionOptions,TransactionScopeAsyncFlowOption.Enabled);

            await operation();

            transactionScope.Complete();
        }

        public async Task<T> ExecuteWithTransactionAsync<T>(Func<Task<T>> operation, TransactionScopeOption scopeOption, TransactionOptions transactionOptions)
        {
            using var transactionScope = new TransactionScope(scopeOption, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);

            T result = await operation();

            transactionScope.Complete();

            return result;
        }
    }
}
