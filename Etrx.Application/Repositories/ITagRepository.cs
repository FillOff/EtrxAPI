using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Etrx.Domain.Entities;

namespace Etrx.Application.Repositories
{
    public interface ITagRepository
    {
        Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken);
        Task<List<Tag>> GetAllAsync(CancellationToken cancellationToken);
        Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task CreateAsync(Tag tag, CancellationToken cancellationToken);
        Task UpdateAsync(Tag tag, CancellationToken cancellationToken);
    }
}