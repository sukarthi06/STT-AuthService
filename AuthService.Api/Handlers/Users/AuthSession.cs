namespace AuthService.Api.Handlers.Users;

public record AuthSession(string AccessToken, string RefreshToken, DateTime RefreshTokenExpiresAt);