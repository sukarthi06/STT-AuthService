namespace AuthService.Application.Repos.Users;

public interface IAppUserRepo : IRepo<AppUser>
{
    public Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
