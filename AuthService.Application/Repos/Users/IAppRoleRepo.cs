namespace AuthService.Application.Repos.Users;

public interface IAppRoleRepo : IRepo<AppRole>
{
    Task<IReadOnlyList<string>> GetRoleNamesAsync(UserId userId, CancellationToken cancellationToken);
}