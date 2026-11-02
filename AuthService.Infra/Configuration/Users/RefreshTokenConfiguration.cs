namespace AuthService.Infra.Configuration.Users;

public sealed class RefreshTokenConfiguration
    : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens", "auth");

        builder.HasKey(x => x.RefreshTokenId);

        builder.Property(x => x.RefreshTokenId)
            .HasColumnName("RefreshTokenId")
            .HasConversion(
                id => id.Value,
                value => RefreshTokenId.Of(value))
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasConversion(
                id => id.Value,
                value => UserId.Of(value))
            .IsRequired();

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.TokenHash)
            .HasMaxLength(64)   // SHA-256 hex
            .IsRequired();

        builder.HasIndex(x => x.TokenHash)
            .IsUnique();

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.FamilyId);

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.Property(x => x.FamilyExpiresAt)
            .IsRequired();

        // Two parallel refreshes with the same token: only one can revoke it
        builder.Property(x => x.RevokedAt)
            .IsConcurrencyToken();

        builder.Property(x => x.ReplacedByTokenId)
            .HasConversion(
                id => id!.Value,
                value => RefreshTokenId.Of(value));

        // Inherited properties
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy);

        builder.Property(x => x.LastModifiedAt);

        builder.Property(x => x.LastModifiedBy);

        builder.Property(x => x.RecordStatus);
    }
}