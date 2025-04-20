using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Etrx.Persistence.Configurations;

public class ContestTranslationConfiguration : IEntityTypeConfiguration<ContestTranslation>
{
    public void Configure(EntityTypeBuilder<ContestTranslation> builder)
    {
        builder.HasKey(ct => new { ct.ContestId, ct.LanguageCode });
    }
}
