using Etrx.Application.Exceptions;
using Etrx.Application.Repositories.UnitOfWork;
using Etrx.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Etrx.Persistence.UnitOfWork;

public sealed class TransactionExecutor : ITransactionExecutor
{
    private readonly EtrxDbContext _dbContext;

    public TransactionExecutor(EtrxDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken)
    {
        await using var transaction = _dbContext.Database.IsRelational()
            ? await _dbContext.Database.BeginTransactionAsync(cancellationToken) : null;
        try
        {
            var result = await action();
            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }
            return result;
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ConflictException("The record already exists.");
        }
    }
}
