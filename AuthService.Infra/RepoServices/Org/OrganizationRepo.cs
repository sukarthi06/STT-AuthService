namespace AuthService.Infra.RepoServices.Org;

public class OrganizationRepo(
    ApplicationDbContext dbContext,
    ILogger<OrganizationRepo> logger) : IOrganizationRepo
{
    private static readonly ActivitySource ActivitySource = new("AuthService.OrganizationRepo");

    public async Task<Organization> CreateAsync(Organization entity, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(CreateAsync));

        var existingEntity = await dbContext.Organizations
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Name == entity.Name, cancellationToken);
        
        if (existingEntity != null)
        {
            logger.LogWarning("Organization with name '{Name}' already exists.", entity.Name);
            throw new InvalidOperationException("Organization with the specified name already exists.");
        }

        existingEntity = await dbContext.Organizations
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.LicenseNo == entity.LicenseNo, cancellationToken);

        if (existingEntity != null)
        {
            logger.LogWarning("Organization with license number '{LicenseNo}' already exists.", entity.LicenseNo);
            throw new InvalidOperationException("Organization with the specified license number already exists.");
        }
        
        await dbContext.Organizations.AddAsync(entity, cancellationToken);
        var result = await dbContext.SaveChangesAsync(cancellationToken);
        if(result <= 0)
        {
            logger.LogError("Failed to create organization '{Name}'.", entity.Name);
            throw new Exception("Failed to create organization.");
        }
        else
        {
            logger.LogInformation("Organization '{Name}' created successfully.", entity.Name);
        }

        return entity;
    }

    public async Task<bool> UpdateAsync(Organization entity, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(UpdateAsync));

        var existingEntity = await dbContext.Organizations
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrganizationId == entity.OrganizationId, cancellationToken);
        if (existingEntity == null)
        {
            logger.LogWarning("Organization with ID '{OrganizationId}' not found, update failed.", entity.OrganizationId);
            throw new KeyNotFoundException("Organization not found, update failed.");
        }
        existingEntity.Name = entity.Name;
        existingEntity.LicenseNo = entity.LicenseNo;
        existingEntity.LastModifiedAt = DateTime.UtcNow;

        dbContext.Organizations.Update(existingEntity);
        var result = await dbContext.SaveChangesAsync(cancellationToken);

        if (result <= 0)
        {
            logger.LogError("Failed to update organization '{Name}'.", entity.Name);
            throw new Exception("Failed to update organization.");
        }
        else
        {
            logger.LogInformation("Organization '{Name}' updated successfully.", entity.Name);
        }
        return true;
    }

    public async Task<bool> DeleteAsync(Organization entity, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(DeleteAsync));

        var existingEntity = await dbContext.Organizations
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrganizationId == entity.OrganizationId, cancellationToken);

        if(existingEntity == null)
        {
            logger.LogWarning("Organization with ID '{OrganizationId}' not found, delete failed.", entity.OrganizationId);
            throw new KeyNotFoundException("Organization not found, delete failed.");
        }

        //dbContext.Organizations.Remove(existingEntity);
        existingEntity.LastModifiedAt = DateTime.UtcNow;
        existingEntity.RecordStatus = (int)RecordStatus.Inactive; // Mark as deleted

        dbContext.Organizations.Update(existingEntity);
        var result = await dbContext.SaveChangesAsync(cancellationToken);

        if (result <= 0)
        {
            logger.LogError("Failed to delete organization '{Name}'.", entity.Name);
            throw new Exception("Failed to delete organization.");
        }
        else
        {
            logger.LogInformation("Organization '{Name}' deleted successfully.", entity.Name);
        }

        return true;
    }

    public async Task<IReadOnlyList<Organization>> GetAllAsync(CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(GetAllAsync));

        var result = await dbContext.Organizations
            .AsNoTracking()
            .Where(o => o.RecordStatus == (int)RecordStatus.Active)
            .ToListAsync(cancellationToken);
        return result;
    }

    public async Task<IReadOnlyList<Organization>?> SearchByAsync(
        Expression<Func<Organization, bool>> filter, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(SearchByAsync));

        return await dbContext.Organizations
            .AsNoTracking()
            .Where(filter)
            .Where(o => o.RecordStatus == (int)RecordStatus.Active)
            .ToListAsync(cancellationToken);
    }
}
