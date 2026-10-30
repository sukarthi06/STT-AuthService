namespace AuthService.Infra.RepoServices.Users;

public class AppRoleRepo(
    ApplicationDbContext dbContext,
    ILogger<AppRoleRepo> logger) : IAppRoleRepo
{
    private static readonly ActivitySource ActivitySource = new("AuthService.AppRoleRepo");

    public Task<AppRole> CreateAsync(AppRole entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(AppRole entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<AppRole>> GetAllAsync(CancellationToken cancellationToken)
    {        
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<string>> GetRoleNamesAsync(UserId userId, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(GetRoleNamesAsync));

        var roles = await dbContext.AppUserRoles
            .AsNoTracking()
            .Where(r => r.UserId == userId && r.RecordStatus == (int)RecordStatus.Active)
            .Select(r => r.Role.RoleName)
            .ToListAsync(cancellationToken);

        return roles.Select(r => r.ToString()).ToList();
    }

    public Task<IReadOnlyList<AppRole>?> SearchByAsync(Expression<Func<AppRole, bool>> filter, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(AppRole entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
