namespace AuthService.Infra.Configuration.Users;

public sealed class AppRoleConfiguration
    : IEntityTypeConfiguration<AppRole>
{
    public void Configure(EntityTypeBuilder<AppRole> builder)
    {
        builder.ToTable("AppRoles", "auth");

        builder.HasKey(x => x.RoleID);

        builder.Property(x => x.RoleID)
            .HasColumnName("RoleID")
            .HasConversion(
                id => id.Value,
                value => RoleID.Of(value))
            .IsRequired();

        builder.Property(x => x.RoleName)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.RoleName)
            .IsUnique();

        // Inherited properties
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy);

        builder.Property(x => x.LastModifiedAt);

        builder.Property(x => x.LastModifiedBy);

        builder.Property(x => x.RecordStatus);
    }
}
