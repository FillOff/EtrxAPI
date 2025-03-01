using Etrx.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Etrx.Persistence.Configurations;

public class UserHandleConfiguration : IEntityTypeConfiguration<UserHandle>
{
    public void Configure(EntityTypeBuilder<UserHandle> builder)
    {
        throw new NotImplementedException();
    }
}
