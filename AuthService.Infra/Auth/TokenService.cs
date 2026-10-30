using Azure.Security.KeyVault.Keys.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Infra.Auth;

public class TokenService(
    SigningKeyProvider signingKeyProvider,
    IOptions<JwtOptions> options) : ITokenService
{
    public async Task<AccessTokenResult> CreateAccessTokenAsync(
        UserId userId, OrganizationId? organizationId, IReadOnlyList<string> roles, CancellationToken ct)
    {
        var jwt = options.Value;
        var signingKey = await signingKeyProvider.GetCurrentAsync(ct);

        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(jwt.AccessTokenMinutes);

        var header = new Dictionary<string, object>
        {
            ["alg"] = "RS256",
            ["typ"] = "JWT",
            ["kid"] = signingKey.KeyId
        };

        var payload = new Dictionary<string, object>
        {
            ["iss"] = jwt.Issuer,
            ["aud"] = jwt.Audience,
            ["sub"] = userId.Value.ToString(),
            ["jti"] = Guid.NewGuid().ToString(),
            ["iat"] = now.ToUnixTimeSeconds(),
            ["nbf"] = now.ToUnixTimeSeconds(),
            ["exp"] = expiresAt.ToUnixTimeSeconds(),
            ["role"] = roles
        };

        if (organizationId is not null)
            payload["org"] = organizationId.Value.ToString();

        var unsignedToken = $"{Encode(header)}.{Encode(payload)}";

        // Hashes locally, signs in Key Vault
        var signResult = await signingKey.Client.SignDataAsync(
            SignatureAlgorithm.RS256, Encoding.ASCII.GetBytes(unsignedToken), ct);

        var token = $"{unsignedToken}.{Base64UrlEncoder.Encode(signResult.Signature)}";

        return new AccessTokenResult(token, expiresAt.UtcDateTime);
    }

    private static string Encode(object value) =>
        Base64UrlEncoder.Encode(JsonSerializer.SerializeToUtf8Bytes(value));
}