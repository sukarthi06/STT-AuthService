namespace AuthService.Domain.Entities.Users;

public class RefreshToken : Entity
{
    public RefreshTokenId RefreshTokenId { get; set; } = default!;
    public UserId UserId { get; set; } = default!;
    public string TokenHash { get; set; } = default!;
    public Guid FamilyId { get; set; }
    public DateTime ExpiresAt { get; set; }        // idle window (30 min)
    public DateTime FamilyExpiresAt { get; set; }  // absolute limit (8 h from login)
    public DateTime? RevokedAt { get; set; }
    public RefreshTokenId? ReplacedByTokenId { get; set; }
}