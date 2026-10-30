namespace AuthService.Application.Abstraction;

public interface ITokenService
{
    Task<AccessTokenResult> CreateAccessTokenAsync(
        UserId userId, OrganizationId? organizationId, IReadOnlyList<string> roles, CancellationToken ct);
}
public record AccessTokenResult(string AccessToken, DateTime ExpiresAt);