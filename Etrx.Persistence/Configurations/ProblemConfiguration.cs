using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Etrx.Persistence.Configurations;

public class ProblemConfiguration : IEntityTypeConfiguration<Problem>
{
    public void Configure(EntityTypeBuilder<Problem> builder)
    {
        builder.HasKey(p => new { p.ContestId, p.Index });

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder
            .HasMany(p => p.ProblemTranslations)
            .WithOne()
            .HasForeignKey(pt => new { pt.ContestId, pt.Index });
    }
}
