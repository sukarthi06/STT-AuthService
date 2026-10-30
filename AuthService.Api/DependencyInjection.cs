using AuthService.Api.Common;

namespace AuthService.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddAppServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        #region "CORS"
        var corsOrigins = configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string>() ?? string.Empty;

        var allowedClientOrigins = corsOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);


        services.AddCors(options =>
        {
            options.AddPolicy("AllowedClientOrigins", policy =>
            {
                policy.WithOrigins(allowedClientOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        #endregion

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        // Services
        services.AddScoped<RegistrationHandler>();
        services.AddScoped<LoginHandler>();
        return services;
    }
}
