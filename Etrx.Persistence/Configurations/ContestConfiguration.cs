using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Etrx.Persistence.Configurations;

public class ContestConfiguration : IEntityTypeConfiguration<Contest>
{
    public void Configure(EntityTypeBuilder<Contest> builder)
    {
        builder.HasKey(c => c.ContestId);

        builder.Property(c => c.ContestId)
            .ValueGeneratedNever();

        builder.HasMany(c => c.Problems)
            .WithOne(p => p.Contest)
            .HasForeignKey(p => p.ContestId);
    }
}