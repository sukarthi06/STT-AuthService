namespace AuthService.Infra.Configuration.Org;

public sealed class OrganizationRoleConfiguration
    : IEntityTypeConfiguration<OrganizationRole>
{
    public void Configure(EntityTypeBuilder<OrganizationRole> builder)
    {
        builder.ToTable("OrganizationRoles", "auth");

        builder.HasKey(x => x.RoleId);

        builder.Property(x => x.RoleId)
            .HasConversion(
                id => id.Value,
                value => OrgRoleId.Of(value))
            .IsRequired();

        builder.Property(x => x.OrganizationId)
            .HasConversion(
                id => id.Value,
                value => OrganizationId.Of(value))
            .IsRequired();

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.RoleName)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.RoleName })
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