using Etrx.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Etrx.Persistence.Configurations;

public class ProblemConfiguration : IEntityTypeConfiguration<Problem>
{
    public void Configure(EntityTypeBuilder<Problem> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Ignore(p => p.Difficulty);

        builder
            .HasMany(p => p.ProblemTranslations)
            .WithOne()
            .HasForeignKey(pt => pt.ProblemId);

        builder
            .HasOne(p => p.Contest)
            .WithMany()
            .HasForeignKey(p => p.GuidContestId);

        builder
            .HasIndex(p => new { p.ContestId, p.Index })
            .IsUnique();

        builder
            .HasMany(p => p.Tags)
            .WithMany(t => t.Problems);
    }
}
