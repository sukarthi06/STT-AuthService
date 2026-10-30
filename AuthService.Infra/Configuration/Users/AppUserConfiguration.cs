namespace AuthService.Infra.Configuration.Users;

public sealed class AppUserConfiguration
    : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("AppUsers", "auth");

        builder.HasKey(x => x.UserId);

        builder.Property(x => x.UserId)
            .HasColumnName("UserId")
            .HasConversion(
                id => id.Value,
                value => UserId.Of(value))
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.Pwd)
            .HasColumnName("PasswordHash")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.UserStatus)
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