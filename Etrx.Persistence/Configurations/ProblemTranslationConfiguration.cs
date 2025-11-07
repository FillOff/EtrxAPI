using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Etrx.Persistence.Configurations;

public class ProblemTranslationConfiguration : IEntityTypeConfiguration<ProblemTranslation>
{
    public void Configure(EntityTypeBuilder<ProblemTranslation> builder)
    {
        builder.HasKey(pt => pt.Id);

        builder
            .HasIndex(pt => new { pt.ProblemId, pt.LanguageCode })
            .IsUnique();
    }
}
