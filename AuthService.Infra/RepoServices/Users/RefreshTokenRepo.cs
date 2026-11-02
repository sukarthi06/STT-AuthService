namespace AuthService.Infra.RepoServices.Users;

public class RefreshTokenRepo(
    ApplicationDbContext dbContext,
    ILogger<RefreshTokenRepo> logger) : IRefreshTokenRepo
{
    private static readonly ActivitySource ActivitySource = new("AuthService.RefreshTokenRepo");

    public async Task<RefreshToken> CreateAsync(RefreshToken entity, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(CreateAsync));

        await dbContext.RefreshTokens.AddAsync(entity, cancellationToken);
        var result = await dbContext.SaveChangesAsync(cancellationToken);

        if (result <= 0)
        {
            logger.LogError("Failed to create refresh token for user {UserId}.", entity.UserId);
            throw new Exception("Failed to create refresh token.");
        }

        return entity;
    }

    public async Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(GetByHashAsync));

        return await dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash
                && t.RecordStatus == (int)RecordStatus.Active, cancellationToken);
    }

    // Revoke current + add next in one SaveChanges (atomic)
    public async Task<bool> RotateAsync(RefreshToken current, RefreshToken next, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(RotateAsync));

        var now = DateTime.UtcNow;
        current.RevokedAt = now;
        current.ReplacedByTokenId = next.RefreshTokenId;
        current.LastModifiedAt = now;

        await dbContext.RefreshTokens.AddAsync(next, cancellationToken);

        try
        {
            var result = await dbContext.SaveChangesAsync(cancellationToken);
            if (result <= 0)
            {
                logger.LogError("Failed to rotate refresh token for user {UserId}.", current.UserId);
                throw new Exception("Failed to rotate refresh token.");
            }
        }
        catch (DbUpdateConcurrencyException)
        {
            // Another request rotated this token first
            logger.LogWarning("Refresh token {RefreshTokenId} was already rotated.", current.RefreshTokenId);
            return false;
        }

        return true;
    }

    public async Task RevokeFamilyAsync(Guid familyId, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(RevokeFamilyAsync));

        var now = DateTime.UtcNow;
        var revoked = await dbContext.RefreshTokens
            .Where(t => t.FamilyId == familyId && t.RevokedAt == null)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.RevokedAt, now)
                .SetProperty(t => t.LastModifiedAt, now), cancellationToken);

        logger.LogInformation("Revoked {Count} refresh tokens in family {FamilyId}.", revoked, familyId);
    }
}