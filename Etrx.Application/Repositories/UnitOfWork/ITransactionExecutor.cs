namespace Etrx.Application.Repositories.UnitOfWork;

public interface ITransactionExecutor
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken);
}
