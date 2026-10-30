using Azure.Core;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Infra.Auth;

public record PublicJsonWebKey(string Kty, string Use, string Alg, string Kid, string N, string E);
public record PublicKeySet(IReadOnlyList<PublicJsonWebKey> Keys, DateTime CachedUntil);
public record SigningKey(string KeyId, CryptographyClient Client, DateTime CachedUntil);

public class SigningKeyProvider(
    TokenCredential credential,
    IOptions<JwtOptions> options,
    ILogger<SigningKeyProvider> logger)
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    private readonly KeyClient _keyClient = new(new Uri(options.Value.KeyVaultUri), credential);
    private readonly SemaphoreSlim _lock = new(1, 1);
    private SigningKey? _current;
    private PublicKeySet? _publicKeys;

    public async Task<SigningKey> GetCurrentAsync(CancellationToken ct)
    {
        var current = _current;
        if (current is not null && DateTime.UtcNow < current.CachedUntil)
            return current;

        await _lock.WaitAsync(ct);
        try
        {
            // Another request may have refreshed it while we waited
            if (_current is not null && DateTime.UtcNow < _current.CachedUntil)
                return _current;

            KeyVaultKey key = await _keyClient.GetKeyAsync(options.Value.KeyName, cancellationToken: ct);

            if (_current?.KeyId != key.Properties.Version)
                _publicKeys = null;

            _current = new SigningKey(
                key.Properties.Version,                        // used as the token's "kid"
                new CryptographyClient(key.Id, credential),    // signs remotely, key never leaves the vault
                DateTime.UtcNow.Add(CacheDuration));

            logger.LogInformation("Signing key version {KeyVersion} loaded.", key.Properties.Version);
            return _current;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<IReadOnlyList<PublicJsonWebKey>> GetPublicKeysAsync(CancellationToken ct)
    {
        var cached = _publicKeys;
        if (cached is not null && DateTime.UtcNow < cached.CachedUntil)
            return cached.Keys;

        await _lock.WaitAsync(ct);
        try
        {
            if (_publicKeys is not null && DateTime.UtcNow < _publicKeys.CachedUntil)
                return _publicKeys.Keys;

            var keys = new List<PublicJsonWebKey>();

            // Every enabled, non-expired version, so tokens signed before a rotation still validate
            await foreach (var version in _keyClient.GetPropertiesOfKeyVersionsAsync(options.Value.KeyName, ct))
            {
                if (version.Enabled != true)
                    continue;
                if (version.ExpiresOn is not null && version.ExpiresOn <= DateTimeOffset.UtcNow)
                    continue;

                KeyVaultKey key = await _keyClient.GetKeyAsync(version.Name, version.Version, ct);

                keys.Add(new PublicJsonWebKey(
                    "RSA", "sig", "RS256", version.Version,
                    Base64UrlEncoder.Encode(key.Key.N),
                    Base64UrlEncoder.Encode(key.Key.E)));
            }

            _publicKeys = new PublicKeySet(keys, DateTime.UtcNow.Add(CacheDuration));
            return keys;
        }
        finally
        {
            _lock.Release();
        }
    }
}