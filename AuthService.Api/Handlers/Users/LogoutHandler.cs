using AuthService.Application.Abstraction;

namespace AuthService.Api.Handlers.Users;

public class LogoutHandler(
    IRefreshTokenRepo refreshTokenRepo,
    ITokenService tokenService,
    ILogger<LogoutHandler> logger)
{
    public async Task HandleAsync(string rawRefreshToken, CancellationToken ct)
    {
        var current = await refreshTokenRepo.GetByHashAsync(tokenService.HashRefreshToken(rawRefreshToken), ct);
        if (current is null)
            return;

        await refreshTokenRepo.RevokeFamilyAsync(current.FamilyId, ct);
        logger.LogInformation("User {UserId} logged out.", current.UserId);
    }
}