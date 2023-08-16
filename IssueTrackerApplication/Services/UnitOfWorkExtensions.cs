using System.Transactions;

namespace IssueTracker.Application.Services
{
    public static class UnitOfWorkExtensions
    {
        public static Task ExecuteWithTransactionAsync(this IUnitOfWork unitOfWork, Func<Task> operation)
        => unitOfWork.ExecuteWithTransactionAsync(operation, TransactionScopeOption.Required, new TransactionOptions());

        public static Task<T> ExecuteWithTransactionAsync<T>(this IUnitOfWork unitOfWork, Func<Task<T>> operation)
            => unitOfWork.ExecuteWithTransactionAsync(operation, TransactionScopeOption.Required, new TransactionOptions());

        public static Task ExecuteWithTransactionAsync(this IUnitOfWork unitOfWork, Func<Task> operation, TransactionScopeOption scopeOption)
            => unitOfWork.ExecuteWithTransactionAsync(operation, scopeOption, new TransactionOptions());

        public static Task<T> ExecuteWithTransactionAsync<T>(this IUnitOfWork unitOfWork, Func<Task<T>> operation, TransactionScopeOption scopeOption)
            => unitOfWork.ExecuteWithTransactionAsync(operation, scopeOption, new TransactionOptions());
    }
}
