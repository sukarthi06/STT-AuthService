using AuthService.Application.Abstraction;
using AuthService.Domain.Enums;

namespace AuthService.Api.Handlers.Users;

public class RefreshHandler(
    IAppUserRepo appUserRepo,
    IRefreshTokenRepo refreshTokenRepo,
    ITokenService tokenService,
    AuthSessionFactory sessionFactory,
    ILogger<RefreshHandler> logger)
{
    // Parallel refreshes (e.g., two tabs) can present a token rotated a moment ago; don't treat that as theft
    private static readonly TimeSpan ReuseGracePeriod = TimeSpan.FromSeconds(10);

    public async Task<AuthSession?> HandleAsync(string rawRefreshToken, CancellationToken ct)
    {
        var current = await refreshTokenRepo.GetByHashAsync(tokenService.HashRefreshToken(rawRefreshToken), ct);
        if (current is null)
        {
            logger.LogWarning("Refresh failed: token not found.");
            return null;
        }

        var now = DateTime.UtcNow;

        if (current.RevokedAt is not null)
        {
            if (now - current.RevokedAt.Value < ReuseGracePeriod)
            {
                logger.LogInformation("Refresh token for User {UserId} was just rotated; ignoring.", current.UserId);
                return null;
            }

            logger.LogWarning("Refresh token reuse detected for User {UserId}, family {FamilyId}. Revoking family.",
                current.UserId, current.FamilyId);
            await refreshTokenRepo.RevokeFamilyAsync(current.FamilyId, ct);
            return null;
        }

        if (current.ExpiresAt <= now || current.FamilyExpiresAt <= now)
        {
            logger.LogInformation("Refresh failed for User {UserId}: session expired.", current.UserId);
            await refreshTokenRepo.RevokeFamilyAsync(current.FamilyId, ct);
            return null;
        }

        // Suspended or deactivated users lose their session at the next refresh
        var users = await appUserRepo.SearchByAsync(
            x => x.UserId == current.UserId && x.UserStatus == UserStatus.Active, ct);
        if (users is not { Count: > 0 })
        {
            logger.LogWarning("Refresh failed for User {UserId}: user not active.", current.UserId);
            await refreshTokenRepo.RevokeFamilyAsync(current.FamilyId, ct);
            return null;
        }

        var accessToken = await sessionFactory.CreateAccessTokenAsync(current.UserId, ct);
        var (next, rawNext) = sessionFactory.CreateRefreshToken(
            current.UserId, current.FamilyId, current.FamilyExpiresAt);

        if (!await refreshTokenRepo.RotateAsync(current, next, ct))
            return null;

        logger.LogInformation("Session refreshed for User {UserId}.", current.UserId);

        return new AuthSession(accessToken, rawNext, next.ExpiresAt);
    }
}