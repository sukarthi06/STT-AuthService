using AuthService.Application.Abstraction;
using AuthService.Infra.Auth;
using Microsoft.Extensions.Options;

namespace AuthService.Api.Handlers.Users;

public class AuthSessionFactory(
    IUserDetailsRepo userDetailsRepo,
    IAppRoleRepo appRoleRepo,
    ITokenService tokenService,
    IOptions<JwtOptions> options)
{
    // Roles/org are re-read on every refresh, so role changes apply within one access-token lifetime
    public async Task<string> CreateAccessTokenAsync(UserId userId, CancellationToken ct)
    {
        var userDetails = await userDetailsRepo.GetByUserIdAsync(userId, ct);
        var roles = await appRoleRepo.GetRoleNamesAsync(userId, ct);

        var token = await tokenService.CreateAccessTokenAsync(userId, userDetails?.OrganizationId, roles, ct);
        return token.AccessToken;
    }

    public DateTime NewFamilyExpiresAt() =>
        DateTime.UtcNow.AddHours(options.Value.RefreshTokenAbsoluteHours);

    public (RefreshToken Entity, string RawToken) CreateRefreshToken(
        UserId userId, Guid familyId, DateTime familyExpiresAt)
    {
        var refresh = tokenService.CreateRefreshToken();
        var idleExpiresAt = DateTime.UtcNow.AddMinutes(options.Value.RefreshTokenIdleMinutes);

        var entity = new RefreshToken
        {
            RefreshTokenId = RefreshTokenId.Of(Guid.NewGuid()),
            UserId = userId,
            TokenHash = refresh.TokenHash,
            FamilyId = familyId,
            FamilyExpiresAt = familyExpiresAt,
            ExpiresAt = idleExpiresAt < familyExpiresAt ? idleExpiresAt : familyExpiresAt
        };

        return (entity, refresh.Token);
    }
}