namespace AuthService.Application.Repos.Users;

public interface IUserDetailsRepo : IRepo<UserDetails>
{
    Task<UserDetails?> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken);
}