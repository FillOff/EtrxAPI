using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Etrx.Persistence.Configurations;

public class ProblemResultConfiguration : IEntityTypeConfiguration<ProblemResult>
{
    public void Configure(EntityTypeBuilder<ProblemResult> builder)
    {
        builder.HasKey(pr => new { pr.Handle, pr.ContestId, pr.Index, pr.ParticipantType });

        builder
            .HasOne<RanklistRow>()
            .WithMany(rr => rr.ProblemResults)
            .HasForeignKey(pr => new { pr.Handle, pr.ContestId, pr.ParticipantType });
    }
}
