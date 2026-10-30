using AuthService.Infra.Auth;
using Microsoft.Extensions.Options;

namespace AuthService.Api.Endpoints;

public static class WellKnownServices
{
    public static void MapWellKnownEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/.well-known")
            .WithTags("Well-Known Endpoints");

        group.MapGet("/jwks.json", GetJwksAsync)
            .WithSummary("Public signing keys used to validate access tokens.");

        group.MapGet("/openid-configuration", GetDiscovery)
            .WithSummary("Discovery document pointing to the JWKS.");
    }

    private static async Task<IResult> GetJwksAsync(
        SigningKeyProvider signingKeyProvider,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var keys = await signingKeyProvider.GetPublicKeysAsync(ct);

        httpContext.Response.Headers.CacheControl = "public, max-age=300";
        return Results.Ok(new { keys });
    }

    private static IResult GetDiscovery(IOptions<JwtOptions> options)
    {
        var issuer = options.Value.Issuer.TrimEnd('/');

        return Results.Ok(new
        {
            issuer = options.Value.Issuer,
            jwks_uri = $"{issuer}/.well-known/jwks.json",
            id_token_signing_alg_values_supported = new[] { "RS256" }
        });
    }
}