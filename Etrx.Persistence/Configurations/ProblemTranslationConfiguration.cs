using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Etrx.Persistence.Configurations;

public class ProblemTranslationConfiguration : IEntityTypeConfiguration<ProblemTranslation>
{
    public void Configure(EntityTypeBuilder<ProblemTranslation> builder)
    {
        builder.HasKey(p => p.Id);

        builder
            .HasIndex(p => new { p.ProblemId, p.LanguageCode })
            .IsUnique();
    }
}
