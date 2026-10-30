namespace AuthService.Infra.Configuration.Org;

public sealed class OrganizationDeptConfiguration
    : IEntityTypeConfiguration<OrganizationDept>
{
    public void Configure(EntityTypeBuilder<OrganizationDept> builder)
    {
        builder.ToTable("OrganizationDepts", "auth");

        builder.HasKey(x => x.DeptId);

        builder.Property(x => x.DeptId)
            .HasColumnName("DeptId")
            .HasConversion(
                id => id.Value,
                value => DepartmentId.Of(value))
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
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

        builder.HasIndex(x => new { x.OrganizationId, x.Name })
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