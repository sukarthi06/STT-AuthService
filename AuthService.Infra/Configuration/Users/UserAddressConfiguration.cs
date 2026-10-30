namespace AuthService.Infra.Configuration.Users;

public sealed class UserAddressConfiguration
    : IEntityTypeConfiguration<UserAddress>
{
    public void Configure(EntityTypeBuilder<UserAddress> builder)
    {
        builder.ToTable("UserAddresses", "auth");

        builder.HasKey(x => x.AddressId);

        builder.Property(x => x.AddressId)
            .HasColumnName("AddressId")
            .UseIdentityAlwaysColumn();

        builder.Property(x => x.UserId)
            .HasColumnName("UserId")
            .HasConversion(
                id => id.Value,
                value => UserId.Of(value))
            .IsRequired();

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId);

        builder.Property(x => x.AddressLine1)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.AddressLine2)
            .HasMaxLength(200);

        builder.Property(x => x.City)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.State)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Country)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Zip)
            .HasMaxLength(20)
            .IsRequired();

        // Inherited properties
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy);

        builder.Property(x => x.LastModifiedAt);

        builder.Property(x => x.LastModifiedBy);

        builder.Property(x => x.RecordStatus);
    }
}