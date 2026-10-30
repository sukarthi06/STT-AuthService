namespace AuthService.Infra.RepoServices.Users;

public class UserAddressRepo(
    ApplicationDbContext dbContext,
    ILogger<UserAddressRepo> logger) : IUserAddressRepo
{
    private static readonly ActivitySource ActivitySource = new("AuthService.UserAddressRepo");

    public async Task<UserAddress> CreateAsync(UserAddress entity, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(CreateAsync));

        var userExists = await dbContext.AppUsers
            .AnyAsync(u => u.UserId == entity.UserId && u.RecordStatus == (int)RecordStatus.Active, cancellationToken);
        if (!userExists)
        {
            logger.LogWarning("User with ID {UserId} not found, address not created.", entity.UserId);
            throw new KeyNotFoundException("User not found, address not created.");
        }

        await dbContext.UserAddresses.AddAsync(entity, cancellationToken);
        var result = await dbContext.SaveChangesAsync(cancellationToken);

        if (result <= 0)
        {
            logger.LogError("Failed to create address for user {UserId}.", entity.UserId);
            throw new Exception("Failed to create address.");
        }
        else
        {
            logger.LogInformation("Address {AddressId} created for user {UserId}.", entity.AddressId, entity.UserId);
        }

        return entity;
    }

    public async Task<bool> UpdateAsync(UserAddress entity, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(UpdateAsync));

        var existingEntity = await dbContext.UserAddresses
            .FirstOrDefaultAsync(a => a.AddressId == entity.AddressId
                && a.RecordStatus == (int)RecordStatus.Active, cancellationToken);
        if (existingEntity == null)
        {
            logger.LogWarning("Address {AddressId} not found, update failed.", entity.AddressId);
            throw new KeyNotFoundException("Address not found, update failed.");
        }

        existingEntity.AddressLine1 = entity.AddressLine1;
        existingEntity.AddressLine2 = entity.AddressLine2;
        existingEntity.City = entity.City;
        existingEntity.State = entity.State;
        existingEntity.Country = entity.Country;
        existingEntity.Zip = entity.Zip;
        existingEntity.LastModifiedAt = DateTime.UtcNow;

        var result = await dbContext.SaveChangesAsync(cancellationToken);

        if (result <= 0)
        {
            logger.LogError("Failed to update address {AddressId}.", entity.AddressId);
            throw new Exception("Failed to update address.");
        }
        else
        {
            logger.LogInformation("Address {AddressId} updated successfully.", entity.AddressId);
        }

        return true;
    }

    public async Task<bool> DeleteAsync(UserAddress entity, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(DeleteAsync));

        var existingEntity = await dbContext.UserAddresses
            .FirstOrDefaultAsync(a => a.AddressId == entity.AddressId
                && a.RecordStatus == (int)RecordStatus.Active, cancellationToken);
        if (existingEntity == null)
        {
            logger.LogWarning("Address {AddressId} not found, delete failed.", entity.AddressId);
            throw new KeyNotFoundException("Address not found, delete failed.");
        }

        existingEntity.RecordStatus = (int)RecordStatus.Inactive;
        existingEntity.LastModifiedAt = DateTime.UtcNow;

        var result = await dbContext.SaveChangesAsync(cancellationToken);

        if (result <= 0)
        {
            logger.LogError("Failed to delete address {AddressId}.", entity.AddressId);
            throw new Exception("Failed to delete address.");
        }
        else
        {
            logger.LogInformation("Address {AddressId} deleted successfully.", entity.AddressId);
        }

        return true;
    }

    public async Task<IReadOnlyList<UserAddress>> GetAllAsync(CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(GetAllAsync));

        return await dbContext.UserAddresses
            .AsNoTracking()
            .Where(a => a.RecordStatus == (int)RecordStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserAddress>?> SearchByAsync(
        Expression<Func<UserAddress, bool>> filter, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(SearchByAsync));

        return await dbContext.UserAddresses
            .AsNoTracking()
            .Where(filter)
            .Where(a => a.RecordStatus == (int)RecordStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserAddress>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(GetByUserIdAsync));

        return await dbContext.UserAddresses
            .AsNoTracking()
            .Where(a => a.UserId == userId && a.RecordStatus == (int)RecordStatus.Active)
            .ToListAsync(cancellationToken);
    }
}