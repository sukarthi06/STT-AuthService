namespace AuthService.Infra.Configuration.Users;

public sealed class UserDetailsConfiguration
    : IEntityTypeConfiguration<UserDetails>
{
    public void Configure(EntityTypeBuilder<UserDetails> builder)
    {
        builder.ToTable("UserDetails", "auth");

        builder.HasKey(x => x.UserId);

        builder.Property(x => x.UserId)
            .HasColumnName("UserId")
            .HasConversion(
                id => id.Value,
                value => UserId.Of(value))
            .IsRequired();

        builder.HasOne<AppUser>()
            .WithOne()
            .HasForeignKey<UserDetails>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.MiddleName)
            .HasMaxLength(100);

        builder.Property(x => x.DOB)
            .HasColumnType("date");

        builder.Property(x => x.Phone1)
            .HasMaxLength(20);

        builder.Property(x => x.Phone2)
            .HasMaxLength(20);

        builder.Property(x => x.Sex);

        builder.Property(x => x.LicenseNo)
            .HasMaxLength(50);

        builder.Property(x => x.OrganizationId)
            .HasConversion(
                id => id!.Value,
                value => OrganizationId.Of(value));

        builder.HasOne(x => x.Organization)
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.OrganizationId);

        // Inherited properties
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy);

        builder.Property(x => x.LastModifiedAt);

        builder.Property(x => x.LastModifiedBy);

        builder.Property(x => x.RecordStatus);
    }
}