using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Etrx.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Etrx.Persistence.Databases;

public class EtrxDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public EtrxDbContext(DbContextOptions<EtrxDbContext> options)
        : base(options)
    { }

    public DbSet<Problem> Problems { get; set; }
    public DbSet<ProblemTranslation> ProblemTranslations { get; set; }
    public DbSet<Contest> Contests { get; set; }
    public DbSet<ContestTranslation> ContestTranslations { get; set; }
    public DbSet<User> Profiles { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<RanklistRow> RanklistRows { get; set; }
    public DbSet<ProblemResult> ProblemResults { get; set; }
    public DbSet<Tag> Tags { get; set; }

    // Retained only as the migration archive; authentication uses Identity's Users.
    public DbSet<UserCredential> UserCredentials { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<UserProblemProgress> UserProblemProgress { get; set; }
    public DbSet<UserProblemSync> UserProblemSync { get; set; }
    public DbSet<UserProblemPreferences> UserProblemPreferences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<ApplicationUser>().ToTable("identity_users");
        modelBuilder.Entity<IdentityRole<Guid>>().ToTable("identity_roles");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("identity_user_roles");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("identity_user_claims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("identity_user_logins");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("identity_user_tokens");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("identity_role_claims");
        modelBuilder.Entity<ApplicationUser>().HasOne<User>().WithOne()
            .HasForeignKey<ApplicationUser>(user => user.Id).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(token => token.TokenHash);
            entity.Property(token => token.TokenHash).HasMaxLength(RefreshToken.HashLength);
            entity.Property(token => token.SecurityStamp).IsRequired();
            entity.Property(token => token.Version).IsConcurrencyToken();
            entity.HasIndex(token => token.ExpiresAt);
            entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(token => token.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
