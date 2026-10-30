using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infra.Extensions;

public static class DatabaseExtentions
{
    public static async Task InitialiseDatabaseAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.Database.MigrateAsync();
    }
}
