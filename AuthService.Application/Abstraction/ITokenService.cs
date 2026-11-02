namespace AuthService.Application.Abstraction;

public interface ITokenService
{
    Task<AccessTokenResult> CreateAccessTokenAsync(
        UserId userId, OrganizationId? organizationId, IReadOnlyList<string> roles, CancellationToken ct);
    RefreshTokenResult CreateRefreshToken();
    string HashRefreshToken(string token);
}
public record AccessTokenResult(string AccessToken, DateTime ExpiresAt);
public record RefreshTokenResult(string Token, string TokenHash);