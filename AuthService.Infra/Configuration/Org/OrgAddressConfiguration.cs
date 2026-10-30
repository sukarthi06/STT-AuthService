namespace AuthService.Infra.Configuration.Org;

public sealed class OrgAddressConfiguration
    : IEntityTypeConfiguration<OrgAddress>
{
    public void Configure(EntityTypeBuilder<OrgAddress> builder)
    {
        builder.ToTable("OrgAddresses", "auth");

        builder.HasKey(x => x.OrganizationId);

        builder.Property(x => x.OrganizationId)
            .HasColumnName("OrganizationId")
            .HasConversion(
                id => id.Value,
                value => OrganizationId.Of(value))
            .IsRequired();

        builder.HasOne<Organization>()
            .WithOne()
            .HasForeignKey<OrgAddress>(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.AddressLine1)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.AddressLine2)
            .HasMaxLength(200);

        builder.Property(x => x.City)
            .HasMaxLength(100);

        builder.Property(x => x.State)
            .HasMaxLength(100);

        builder.Property(x => x.Country)
            .HasMaxLength(100);

        builder.Property(x => x.Zip)
            .HasMaxLength(20);

        builder.Property(x => x.Phone1)
            .HasMaxLength(20);

        builder.Property(x => x.Phone2)
            .HasMaxLength(20);

        builder.Property(x => x.PrimaryUserId)
            .HasConversion(
                id => id!.Value,
                value => UserId.Of(value));

        builder.Property(x => x.SecondaryUserId)
            .HasConversion(
                id => id!.Value,
                value => UserId.Of(value));

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.PrimaryUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.SecondaryUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.PrimaryUserId);

        builder.HasIndex(x => x.SecondaryUserId);

        // Inherited properties
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy);

        builder.Property(x => x.LastModifiedAt);

        builder.Property(x => x.LastModifiedBy);

        builder.Property(x => x.RecordStatus);
    }
}