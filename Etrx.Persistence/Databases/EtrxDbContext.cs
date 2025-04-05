﻿using Etrx.Core.Models;
using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Etrx.Persistence.Databases
{
    public class EtrxDbContext : DbContext
    {
        public EtrxDbContext(DbContextOptions<EtrxDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Problem> Problems { get; set; }
        public DbSet<ProblemTranslation> ProblemTranslations { get; set; }
        public DbSet<Contest> Contests { get; set; }
        public DbSet<ContestTranslation> ContestTranslations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Submission> Submissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
