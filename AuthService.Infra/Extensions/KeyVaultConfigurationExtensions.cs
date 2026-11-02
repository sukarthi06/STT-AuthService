using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace AuthService.Infra.Extensions;

public static class KeyVaultConfigurationExtensions
{
    public static IConfigurationManager AddGatewaySecrets(this IConfigurationManager configuration)
    {
        var vaultUri = configuration["Jwt:KeyVaultUri"]
            ?? throw new InvalidOperationException("Jwt:KeyVaultUri is not configured.");

        configuration.AddAzureKeyVault(
            new Uri(vaultUri),
            new DefaultAzureCredential(),
            new AzureKeyVaultConfigurationOptions
            {
                Manager = new PrefixKeyVaultSecretManager("GatewayAuth--"),
                ReloadInterval = TimeSpan.FromMinutes(5)
            });

        return configuration;
    }

    private sealed class PrefixKeyVaultSecretManager(string prefix) : KeyVaultSecretManager
    {
        public override bool Load(SecretProperties secret) =>
            secret.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }
}