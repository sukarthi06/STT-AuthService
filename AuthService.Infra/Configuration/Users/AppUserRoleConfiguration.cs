namespace AuthService.Infra.Configuration.Users;

public sealed class AppUserRoleConfiguration
    : IEntityTypeConfiguration<AppUserRole>
{
    public void Configure(EntityTypeBuilder<AppUserRole> builder)
    {
        builder.ToTable("AppUserRoles", "auth");

        builder.HasKey(x => new { x.UserId, x.RoleId });

        builder.Property(x => x.UserId)
            .HasColumnName("UserId")
            .HasConversion(
                id => id.Value,
                value => UserId.Of(value))
            .IsRequired();

        builder.Property(x => x.RoleId)
            .HasColumnName("RoleID")
            .HasConversion(
                id => id.Value,
                value => RoleID.Of(value))
            .IsRequired();

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.RoleId);

        // Inherited properties
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy);

        builder.Property(x => x.LastModifiedAt);

        builder.Property(x => x.LastModifiedBy);

        builder.Property(x => x.RecordStatus);
    }
}