namespace AuthService.Api.Handlers.Users;

public class LoginHandler(
    IAppUserRepo appUserRepo,
    IRefreshTokenRepo refreshTokenRepo,
    IPasswordHasher<AppUser> passwordHasher,
    AuthSessionFactory sessionFactory,
    ILogger<LoginHandler> logger)
{
    public async Task<AuthSession?> HandleAsync(LoginDto loginDto, CancellationToken ct)
    {
        var user = await appUserRepo.GetByEmailAsync(loginDto.Email, ct);
        if (user is null)
        {
            logger.LogWarning("Login failed: user not found or inactive.");
            return null;
        }

        var result = passwordHasher.VerifyHashedPassword(user, user.Pwd, loginDto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            logger.LogWarning("Login failed for User {UserId}: invalid password.", user.UserId);
            return null;
        }

        var accessToken = await sessionFactory.CreateAccessTokenAsync(user.UserId, ct);

        var (refreshToken, rawRefreshToken) = sessionFactory.CreateRefreshToken(
            user.UserId, Guid.NewGuid(), sessionFactory.NewFamilyExpiresAt());
        await refreshTokenRepo.CreateAsync(refreshToken, ct);

        logger.LogInformation("User {UserId} logged in.", user.UserId);

        return new AuthSession(accessToken, rawRefreshToken, refreshToken.ExpiresAt);
    }
}