using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Etrx.Persistence.Configurations;

public class RanklistRowConfiguration : IEntityTypeConfiguration<RanklistRow>
{
    public void Configure(EntityTypeBuilder<RanklistRow> builder)
    {
        builder.HasKey(rr => rr.Id);

        builder
            .HasMany(rr => rr.ProblemResults)
            .WithOne()
            .HasForeignKey(pr => new { pr.RanklistRowId });

        builder
            .HasIndex(rr => new { rr.Handle, rr.ContestId, rr.ParticipantType })
            .IsUnique();
    }
}
