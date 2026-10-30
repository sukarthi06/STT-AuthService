namespace AuthService.Infra.Configuration.Org;

public sealed class OrgUserRoleConfiguration
    : IEntityTypeConfiguration<OrgUserRole>
{
    public void Configure(EntityTypeBuilder<OrgUserRole> builder)
    {
        builder.ToTable("OrgUserRoles", "auth", t =>
        {
            t.HasCheckConstraint("CK_OrgUserRoles_Dept",
                "NOT (\"AllDepartments\" AND \"DepartmentId\" IS NOT NULL)");
            t.HasCheckConstraint("CK_OrgUserRoles_Role",
                "NOT (\"AllRoles\" AND \"OrgRoleId\" IS NOT NULL)");
        });

        builder.HasKey(x => new { x.OrganizationId, x.UserId });

        builder.Property(x => x.OrganizationId)
            .HasConversion(
                id => id.Value,
                value => OrganizationId.Of(value))
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasConversion(
                id => id.Value,
                value => UserId.Of(value))
            .IsRequired();

        builder.Property(x => x.DepartmentId)
            .HasConversion(
                id => id!.Value,
                value => DepartmentId.Of(value));

        builder.Property(x => x.OrgRoleId)
            .HasConversion(
                id => id!.Value,
                value => OrgRoleId.Of(value));

        builder.Property(x => x.AllDepartments)
            .IsRequired();

        builder.Property(x => x.AllRoles)
            .IsRequired();

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Composite FKs: the department and role must belong to the same organization.
        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => new { x.OrganizationId, x.DepartmentId })
            .HasPrincipalKey(d => new { d.OrganizationId, d.DeptId })
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey(x => new { x.OrganizationId, x.OrgRoleId })
            .HasPrincipalKey(r => new { r.OrganizationId, r.RoleId })
            .OnDelete(DeleteBehavior.NoAction);

        // Inherited properties
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy);

        builder.Property(x => x.LastModifiedAt);

        builder.Property(x => x.LastModifiedBy);

        builder.Property(x => x.RecordStatus);

        builder.HasIndex(x => x.UserId);
    }
}