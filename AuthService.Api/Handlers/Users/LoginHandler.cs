using AuthService.Application.Abstraction;

namespace AuthService.Api.Handlers.Users;

public class LoginHandler(
    IAppUserRepo appUserRepo,
    IAppRoleRepo appRoleRepo,
    IUserDetailsRepo userDetailsRepo,
    IPasswordHasher<AppUser> passwordHasher,
    ITokenService tokenService,
    ILogger<LoginHandler> logger)
{
    public async Task<LoginResponseDto?> HandleAsync(LoginDto loginDto, CancellationToken ct)
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

        var userDetails = await userDetailsRepo.GetByUserIdAsync(user.UserId, ct);
        var roles = await appRoleRepo.GetRoleNamesAsync(user.UserId, ct);

        var token = await tokenService.CreateAccessTokenAsync(
            user.UserId, userDetails?.OrganizationId, roles, ct);

        logger.LogInformation("User {UserId} logged in.", user.UserId);

        return new LoginResponseDto(token.AccessToken);
    }
}