namespace AuthService.Application.Repos.Users;

public interface IUserAddressRepo : IRepo<UserAddress>
{
    Task<IReadOnlyList<UserAddress>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken);
}