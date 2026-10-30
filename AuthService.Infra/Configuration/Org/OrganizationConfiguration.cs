namespace AuthService.Infra.Configuration.Org;

public sealed class OrganizationConfiguration
    : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations", "auth");

        builder.HasKey(x => x.OrganizationId);

        builder.Property(x => x.OrganizationId)
            .HasColumnName("OrganizationId")
            .HasConversion(
                id => id.Value,
                value => OrganizationId.Of(value))
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.LicenseNo)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.LicenseNo)
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