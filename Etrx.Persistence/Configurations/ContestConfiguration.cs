using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Etrx.Persistence.Configurations;

public class ContestConfiguration : IEntityTypeConfiguration<Contest>
{
    public void Configure(EntityTypeBuilder<Contest> builder)
    {
        builder.HasKey(c => c.Id);

        builder
            .HasMany(c => c.ContestTranslations)
            .WithOne()
            .HasForeignKey(ct => ct.ContestId);

        builder
            .HasIndex(c => c.ContestId)
            .IsUnique();
    }
}