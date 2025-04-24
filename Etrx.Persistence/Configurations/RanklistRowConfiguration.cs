using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Etrx.Persistence.Configurations;

public class RanklistRowConfiguration : IEntityTypeConfiguration<RanklistRow>
{
    public void Configure(EntityTypeBuilder<RanklistRow> builder)
    {
        builder.HasKey(rr => new { rr.Handle, rr.ContestId, rr.ParticipantType });

        builder
            .HasMany(rr => rr.ProblemResults)
            .WithOne()
            .HasForeignKey(pr => new { pr.Handle, pr.ContestId, pr.ParticipantType });
    }
}
