namespace AuthService.Api.Handlers.Users;

public class RegistrationHandler(
    IAppUserRepo appUserRepo,
    IUserDetailsRepo userDetailsRepo,
    IOrganizationRepo organizationRepo,
    IPasswordHasher<AppUser> passwordHasher,
    RegistrationMapper mapper,
    ILogger<RegistrationHandler> logger)
{
    public async Task<UserId?> HandleAsync(RegistrationDto registrationDto, CancellationToken ct)
    {
        var email = registrationDto.Email.Trim().ToLowerInvariant();

        var existingUser = await appUserRepo.SearchByAsync(x => x.Email == email, ct);
        if (existingUser is { Count: > 0 })
        {
            logger.LogWarning("Registration failed, email already exists");
            return null;
        }

        var appUser = mapper.ToAppUser(registrationDto, passwordHasher.HashPassword(null!, registrationDto.Password));
        appUser.Email = email;
        appUser = await appUserRepo.CreateAsync(appUser, ct);

        var userDetails = mapper.ToUserDetails(registrationDto);
        userDetails.UserId = appUser.UserId;

        if (!string.IsNullOrWhiteSpace(registrationDto.OrganizationName))
        {
            var organization = await GetOrCreateOrganizationAsync(registrationDto, ct);
            userDetails.OrganizationId = organization.OrganizationId;
        }

        await userDetailsRepo.CreateAsync(userDetails, ct);

        logger.LogInformation("User: {UserId} registered", appUser.UserId);

        return appUser.UserId;
    }

    private async Task<Organization> GetOrCreateOrganizationAsync(RegistrationDto registrationDto, CancellationToken ct)
    {
        var existing = await organizationRepo.SearchByAsync(
            x => x.LicenseNo == registrationDto.OrganizationLicenseNo, ct);

        return existing is { Count: > 0 }
            ? existing[0]
            : await organizationRepo.CreateAsync(mapper.ToOrganization(registrationDto), ct);
    }
}