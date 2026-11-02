namespace AuthService.Application.Repos.Users;

public interface IRefreshTokenRepo
{
    Task<RefreshToken> CreateAsync(RefreshToken entity, CancellationToken cancellationToken);
    Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken);
    Task<bool> RotateAsync(RefreshToken current, RefreshToken next, CancellationToken cancellationToken);
    Task RevokeFamilyAsync(Guid familyId, CancellationToken cancellationToken);
}