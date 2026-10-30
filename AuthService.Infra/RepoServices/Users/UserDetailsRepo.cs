namespace AuthService.Infra.RepoServices.Users;

public class UserDetailsRepo(
    ApplicationDbContext dbContext,
    ILogger<UserDetailsRepo> logger) : IUserDetailsRepo
{
    private static readonly ActivitySource ActivitySource = new("AuthService.UserDetailsRepo");

    public async Task<UserDetails> CreateAsync(UserDetails entity, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(CreateAsync));

        var userExists = await dbContext.AppUsers
            .AnyAsync(u => u.UserId == entity.UserId && u.RecordStatus == (int)RecordStatus.Active, cancellationToken);
        if (!userExists)
        {
            logger.LogWarning("User with ID {UserId} not found, details not created.", entity.UserId);
            throw new KeyNotFoundException("User not found, details not created.");
        }

        var detailsExist = await dbContext.UserDetails
            .AnyAsync(d => d.UserId == entity.UserId, cancellationToken);
        if (detailsExist)
        {
            logger.LogWarning("Details for user {UserId} already exist.", entity.UserId);
            throw new InvalidOperationException("User details already exist.");
        }

        await dbContext.UserDetails.AddAsync(entity, cancellationToken);
        var result = await dbContext.SaveChangesAsync(cancellationToken);

        if (result <= 0)
        {
            logger.LogError("Failed to create details for user {UserId}.", entity.UserId);
            throw new Exception("Failed to create user details.");
        }
        else
        {
            logger.LogInformation("Details created for user {UserId}.", entity.UserId);
        }

        return entity;
    }

    public async Task<bool> UpdateAsync(UserDetails entity, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(UpdateAsync));

        var existingEntity = await dbContext.UserDetails
            .FirstOrDefaultAsync(d => d.UserId == entity.UserId
                && d.RecordStatus == (int)RecordStatus.Active, cancellationToken);
        if (existingEntity == null)
        {
            logger.LogWarning("Details for user {UserId} not found, update failed.", entity.UserId);
            throw new KeyNotFoundException("User details not found, update failed.");
        }

        existingEntity.FirstName = entity.FirstName;
        existingEntity.LastName = entity.LastName;
        existingEntity.MiddleName = entity.MiddleName;
        existingEntity.DOB = entity.DOB;
        existingEntity.Phone1 = entity.Phone1;
        existingEntity.Phone2 = entity.Phone2;
        existingEntity.Sex = entity.Sex;
        existingEntity.LicenseNo = entity.LicenseNo;
        existingEntity.OrganizationId = entity.OrganizationId;
        existingEntity.LastModifiedAt = DateTime.UtcNow;

        var result = await dbContext.SaveChangesAsync(cancellationToken);

        if (result <= 0)
        {
            logger.LogError("Failed to update details for user {UserId}.", entity.UserId);
            throw new Exception("Failed to update user details.");
        }
        else
        {
            logger.LogInformation("Details for user {UserId} updated successfully.", entity.UserId);
        }

        return true;
    }

    public async Task<bool> DeleteAsync(UserDetails entity, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(DeleteAsync));

        var existingEntity = await dbContext.UserDetails
            .FirstOrDefaultAsync(d => d.UserId == entity.UserId
                && d.RecordStatus == (int)RecordStatus.Active, cancellationToken);
        if (existingEntity == null)
        {
            logger.LogWarning("Details for user {UserId} not found, delete failed.", entity.UserId);
            throw new KeyNotFoundException("User details not found, delete failed.");
        }

        existingEntity.RecordStatus = (int)RecordStatus.Inactive;
        existingEntity.LastModifiedAt = DateTime.UtcNow;

        var result = await dbContext.SaveChangesAsync(cancellationToken);

        if (result <= 0)
        {
            logger.LogError("Failed to delete details for user {UserId}.", entity.UserId);
            throw new Exception("Failed to delete user details.");
        }
        else
        {
            logger.LogInformation("Details for user {UserId} deleted successfully.", entity.UserId);
        }

        return true;
    }

    public async Task<IReadOnlyList<UserDetails>> GetAllAsync(CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(GetAllAsync));

        return await dbContext.UserDetails
            .AsNoTracking()
            .Include(d => d.Organization)
            .Where(d => d.RecordStatus == (int)RecordStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserDetails>?> SearchByAsync(
        Expression<Func<UserDetails, bool>> filter, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(SearchByAsync));

        return await dbContext.UserDetails
            .AsNoTracking()
            .Include(d => d.Organization)
            .Where(filter)
            .Where(d => d.RecordStatus == (int)RecordStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserDetails?> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(GetByUserIdAsync));

        return await dbContext.UserDetails
            .AsNoTracking()
            .Include(d => d.Organization)
            .FirstOrDefaultAsync(d => d.UserId == userId
                && d.RecordStatus == (int)RecordStatus.Active, cancellationToken);
    }
}