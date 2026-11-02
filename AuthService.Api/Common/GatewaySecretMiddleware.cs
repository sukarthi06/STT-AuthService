using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace AuthService.Api.Common;

public class GatewaySecretMiddleware(
    RequestDelegate next,
    IOptionsMonitor<GatewayAuthOptions> options,
    ILogger<GatewaySecretMiddleware> logger)
{
    private const string HeaderName = "X-Gateway-Secret";

    // Health probes and public signing keys (gateway's JWKS fetch bypasses YARP)
    private static readonly PathString[] ExemptPaths = ["/health", "/.well-known"];

    public async Task InvokeAsync(HttpContext context)
    {
        if (ExemptPaths.Any(p => context.Request.Path.StartsWithSegments(p)))
        {
            await next(context);
            return;
        }

        var provided = context.Request.Headers[HeaderName].ToString();
        var gatewayAuth = options.CurrentValue;

        if (!IsMatch(provided, gatewayAuth.Secret) && !IsMatch(provided, gatewayAuth.NextSecret))
        {
            logger.LogWarning("Rejected request to {Path}: missing or invalid gateway secret.", context.Request.Path);
            await Results.Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Unauthorized")
                .ExecuteAsync(context);
            return;
        }

        await next(context);
    }

    // Hash first so the comparison is fixed-length and constant-time
    private static bool IsMatch(string provided, string expected)
    {
        if (string.IsNullOrEmpty(provided) || string.IsNullOrEmpty(expected))
            return false;

        return CryptographicOperations.FixedTimeEquals(
            SHA256.HashData(Encoding.UTF8.GetBytes(provided)),
            SHA256.HashData(Encoding.UTF8.GetBytes(expected)));
    }
}