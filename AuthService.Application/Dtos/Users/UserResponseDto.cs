namespace AuthService.Application.Dtos.Users;

public record UserResponseDto(
    Guid UserId,
    string Email,
    string FirstName,
    string? MiddleName,
    string LastName,
    DateTime? DOB,
    string? Phone1,
    string? Phone2,
    string? Sex,
    string? LicenseNo,
    string UserStatus,
    UserOrganizationDto? Organization,
    UserAddressDto? Address,
    IReadOnlyList<string> Roles);
