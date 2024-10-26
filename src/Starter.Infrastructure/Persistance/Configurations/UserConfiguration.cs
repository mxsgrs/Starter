using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Starter.Domain.Aggregates.UserAggregate;

namespace Starter.Infrastructure.Persistance.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.EmailAddress).IsUnique();

        builder.Property(e => e.Role).HasConversion<string>();

        builder.Property(e => e.Gender).HasConversion<string>();

        builder.OwnsOne(x => x.Address, ua =>
        {
            ua.ToTable("UserAddresses");
        });
    }
}
