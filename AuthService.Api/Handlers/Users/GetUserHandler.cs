namespace AuthService.Api.Handlers.Users;

public class GetUserHandler(
    IAppUserRepo appUserRepo,
    IUserDetailsRepo userDetailsRepo,
    IUserAddressRepo userAddressRepo,
    IAppRoleRepo appRoleRepo,
    UserMapper mapper,
    ILogger<GetUserHandler> logger)
{
    public async Task<UserResponseDto?> HandleAsync(UserId userId, CancellationToken ct)
    {
        var users = await appUserRepo.SearchByAsync(x => x.UserId == userId, ct);
        var user = users?.FirstOrDefault();
        if (user is null)
        {
            logger.LogWarning("User {UserId} not found.", userId);
            return null;
        }

        var details = await userDetailsRepo.GetByUserIdAsync(userId, ct);
        if (details is null)
        {
            logger.LogWarning("User details for {UserId} not found.", userId);
            return null;
        }

        var addresses = await userAddressRepo.GetByUserIdAsync(userId, ct);
        var roles = await appRoleRepo.GetRoleNamesAsync(userId, ct);

        logger.LogInformation("User {UserId} retrieved.", userId);

        return new UserResponseDto(
            user.UserId.Value,
            user.Email,
            details.FirstName,
            details.MiddleName,
            details.LastName,
            details.DOB,
            details.Phone1,
            details.Phone2,
            details.Sex?.ToString(),
            details.LicenseNo,
            user.UserStatus.ToString(),
            details.Organization is null ? null : mapper.ToUserOrganizationDto(details.Organization),
            addresses.Count > 0 ? mapper.ToUserAddressDto(addresses[0]) : null,
            roles);
    }
}