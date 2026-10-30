namespace AuthService.Infra.Auth;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public int AccessTokenMinutes { get; set; } = 15;
    public string KeyVaultUri { get; set; } = default!;
    public string KeyName { get; set; } = default!;
}