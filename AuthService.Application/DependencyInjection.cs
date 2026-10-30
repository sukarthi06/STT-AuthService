using AuthService.Application.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Security
        services.AddSingleton<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Mappers
        services.AddSingleton<RegistrationMapper>();        

        return services;
    }
}
