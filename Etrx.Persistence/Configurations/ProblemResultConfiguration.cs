using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Etrx.Persistence.Configurations;

public class ProblemResultConfiguration : IEntityTypeConfiguration<ProblemResult>
{
    public void Configure(EntityTypeBuilder<ProblemResult> builder)
    {
        builder.HasKey(pr => pr.Id);

        builder
            .HasIndex(pr => new { pr.RanklistRowId, pr.Index })
            .IsUnique();
    }
}
