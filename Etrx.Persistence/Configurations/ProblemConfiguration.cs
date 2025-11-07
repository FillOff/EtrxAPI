using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Etrx.Persistence.Configurations;

public class ProblemConfiguration : IEntityTypeConfiguration<Problem>
{
    public void Configure(EntityTypeBuilder<Problem> builder)
    {
        builder.HasKey(ct => ct.Id);

        builder
            .HasMany(p => p.ProblemTranslations)
            .WithOne()
            .HasForeignKey(pt => pt.ProblemId);

        builder
            .HasIndex(p => new { p.ContestId, p.Index })
            .IsUnique();
    }
}
