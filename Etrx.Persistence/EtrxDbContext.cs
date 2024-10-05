using Etrx.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Etrx.Persistence
{
    public class EtrxDbContext : DbContext
    {
        public EtrxDbContext(DbContextOptions<EtrxDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProblemEntity> Problems { get; set; }
    }
}
