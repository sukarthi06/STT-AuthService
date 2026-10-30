namespace AuthService.Application.Dtos.Users;

public record RegistrationDto
(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? LicenseNo,

    string? OrganizationName,
    string? OrganizationLicenseNo
);
