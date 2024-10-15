using Etrx.Domain.Models;
using Etrx.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Etrx.Persistence.Databases
{
    public class EtrxDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public EtrxDbContext(IConfiguration configuration)
        {
            _configuration = configuration;

            Database.EnsureCreated();
        }

        public DbSet<Problem> Problems { get; set; }
        public DbSet<Contest> Contests { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Submission> Submissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProblemConfiguration());
            modelBuilder.ApplyConfiguration(new ContestConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new SubmissionConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString(nameof(EtrxDbContext)));
        }
    }
}
