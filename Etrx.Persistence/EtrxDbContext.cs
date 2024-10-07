using Etrx.Domain.Models;
using Etrx.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Etrx.Persistence
{
    public class EtrxDbContext : DbContext
    {
        public EtrxDbContext(DbContextOptions<EtrxDbContext> options)
            : base(options)
        {
        }

        public DbSet<Problem> Problems { get; set; }
        public DbSet<Contest> Contests { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProblemConfiguration());
            modelBuilder.ApplyConfiguration(new ContestConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
