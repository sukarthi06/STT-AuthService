namespace AuthService.Application.Dtos.Users;

public record UserAddressDto(
    int AddressId,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string State,
    string Country,
    string Zip);
