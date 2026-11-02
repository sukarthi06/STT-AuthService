namespace AuthService.Application.Dtos.Users;

public record UserOrganizationDto(
    Guid OrganizationId,
    string Name);
