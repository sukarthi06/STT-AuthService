namespace AuthService.Infra.RepoServices.Users;

public class AppUserRepo(
    ApplicationDbContext dbContext,
    ILogger<AppUserRepo> logger) : IAppUserRepo
{
    private static readonly ActivitySource ActivitySource = new("AuthService.AppUserRepo");

    public async Task<AppUser> CreateAsync(AppUser entity, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(CreateAsync));

        entity.Email = entity.Email.Trim().ToLowerInvariant();

        var existingEntity = await dbContext.AppUsers.FirstOrDefaultAsync(u => u.Email == entity.Email, cancellationToken);
        if (existingEntity != null)
        {
            logger.LogWarning("User with this email already exists.");
            throw new InvalidOperationException($"User with email {entity.Email} already exists.");
        }

        await dbContext.AppUsers.AddAsync(entity, cancellationToken);
        var result = await dbContext.SaveChangesAsync(cancellationToken);

        if(result <= 0)
        {
            logger.LogError("Failed to create user.");
            throw new Exception($"Failed to create user with email {entity.Email}.");
        }
        else
            logger.LogInformation("User {UserId} created successfully.", entity.UserId);

        return entity;
    }

    public async Task<bool> UpdateAsync(AppUser entity, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(UpdateAsync));

        entity.Email = entity.Email.Trim().ToLowerInvariant();

        var existingEntity = await dbContext.AppUsers.FirstOrDefaultAsync(u => u.UserId == entity.UserId, cancellationToken);
        if (existingEntity == null)
        {
            logger.LogWarning("User with ID {UserId} not found.", entity.UserId);
            throw new KeyNotFoundException($"User with ID {entity.UserId} not found.");
        }

        var checkEmail = await 
            dbContext.AppUsers.FirstOrDefaultAsync(u => u.Email == entity.Email && u.UserId != entity.UserId, cancellationToken);
        if (checkEmail != null)
        {
            logger.LogWarning("This email has already been taken.");
            throw new InvalidOperationException($"This email {entity.Email} has already been taken.");            
        }

        // Update the existing entity with the new values
        existingEntity.Email = entity.Email;
        existingEntity.LastModifiedAt = DateTime.UtcNow;

        dbContext.AppUsers.Update(existingEntity);
        var result = await dbContext.SaveChangesAsync(cancellationToken);
        if (result <= 0)
        {
            logger.LogError("Failed to update user with ID {UserId}.", entity.UserId);
            throw new Exception($"Failed to update user with ID {entity.UserId}.");
        }
        else
        {
            logger.LogInformation("User with ID {UserId} updated successfully.", entity.UserId);
        }

        return true;
    }

    public async Task<bool> DeleteAsync(AppUser entity, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(DeleteAsync));

        var existingEntity = await dbContext.AppUsers.FirstOrDefaultAsync(u => u.UserId == entity.UserId, cancellationToken);

        if (existingEntity == null)
        {
            logger.LogWarning("User with ID {UserId} not found.", entity.UserId);
            throw new KeyNotFoundException($"User with ID {entity.UserId} not found.");
        }

        existingEntity.RecordStatus = (int)RecordStatus.Inactive;
        existingEntity.LastModifiedAt = DateTime.UtcNow;

        dbContext.AppUsers.Update(existingEntity);
        var result = await dbContext.SaveChangesAsync(cancellationToken);

        if (result <= 0)
        {
            logger.LogError("Failed to delete user with ID {UserId}.", entity.UserId);
            throw new Exception($"Failed to delete user with ID {entity.UserId}.");
        }
        else
        {
            logger.LogInformation("User with ID {UserId} deleted successfully.", entity.UserId);
        }

        return true;
    }

    public async Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(GetAllAsync));

        var users = await dbContext.AppUsers
            .AsNoTracking()
            .Where(u => u.RecordStatus == (int)RecordStatus.Active)
            .ToListAsync(cancellationToken);

        return users;
    }

    public async Task<IReadOnlyList<AppUser>?> SearchByAsync(Expression<Func<AppUser, bool>> filter, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(SearchByAsync));

        var users = await dbContext.AppUsers
            .AsNoTracking()
            .Where(filter)
            .Where(u => u.RecordStatus == (int)RecordStatus.Active)
            .ToListAsync(cancellationToken);

        return users;
    }

    public async Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(GetByEmailAsync));

        var user = await dbContext.AppUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email.Trim().ToLowerInvariant()
                && u.RecordStatus == (int)RecordStatus.Active
                && u.UserStatus == UserStatus.Active,
                cancellationToken);

        return user;
    }
}
