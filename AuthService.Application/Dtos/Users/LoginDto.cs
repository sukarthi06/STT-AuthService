namespace AuthService.Application.Dtos.Users;

public record LoginDto(
    string Email,
    string Password);
