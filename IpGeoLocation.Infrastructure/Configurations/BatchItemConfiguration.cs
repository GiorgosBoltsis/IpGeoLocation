using IpGeoLocation.Domain.Batches;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IpGeoLocation.Infrastructure.Persistence.Configurations;

public class BatchItemConfiguration : IEntityTypeConfiguration<BatchItem>
{
    public void Configure(EntityTypeBuilder<BatchItem> builder)
    {
        builder.ToTable("BatchItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Ip)
            .IsRequired()
            .HasMaxLength(45);

        builder.Property(i => i.Status)
            .IsRequired();

        builder.Property(i => i.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(i => i.CountryCode)
            .HasMaxLength(10);

        builder.Property(i => i.CountryName)
            .HasMaxLength(200);

        builder.Property(i => i.TimeZone)
            .HasMaxLength(200);
    }
}
