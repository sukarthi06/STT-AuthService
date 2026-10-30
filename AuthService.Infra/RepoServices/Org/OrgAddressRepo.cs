namespace AuthService.Infra.RepoServices.Org;

public class OrgAddressRepo(
    ApplicationDbContext dbContext,
    ILogger<OrgAddressRepo> logger) : IOrgAddressRepo
{
    private static readonly ActivitySource ActivitySource = new("AuthService.OrgAddressRepo");

    public async Task<OrgAddress> CreateAsync(OrgAddress entity, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(CreateAsync));

        var orgExists = await dbContext.Organizations
            .AnyAsync(o => o.OrganizationId == entity.OrganizationId
                && o.RecordStatus == (int)RecordStatus.Active, cancellationToken);
        if (!orgExists)
        {
            logger.LogWarning("Organization {OrganizationId} not found, address not created.", entity.OrganizationId);
            throw new KeyNotFoundException("Organization not found, address not created.");
        }

        await EnsureContactsAreMembersAsync(entity, cancellationToken);

        await dbContext.OrgAddresses.AddAsync(entity, cancellationToken);
        var result = await dbContext.SaveChangesAsync(cancellationToken);

        if (result <= 0)
        {
            logger.LogError("Failed to create address for organization {OrganizationId}.", entity.OrganizationId);
            throw new Exception("Failed to create organization address.");
        }
        else
        {
            logger.LogInformation("Address created for organization {OrganizationId}.", entity.OrganizationId);
        }

        return entity;
    }

    public async Task<bool> UpdateAsync(OrgAddress entity, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(UpdateAsync));

        var existingEntity = await dbContext.OrgAddresses
            .FirstOrDefaultAsync(a => a.OrganizationId == entity.OrganizationId
                && a.RecordStatus == (int)RecordStatus.Active, cancellationToken);
        if (existingEntity == null)
        {
            logger.LogWarning("Address for organization {OrganizationId} not found, update failed.", entity.OrganizationId);
            throw new KeyNotFoundException("Organization address not found, update failed.");
        }

        await EnsureContactsAreMembersAsync(entity, cancellationToken);

        existingEntity.AddressLine1 = entity.AddressLine1;
        existingEntity.AddressLine2 = entity.AddressLine2;
        existingEntity.City = entity.City;
        existingEntity.State = entity.State;
        existingEntity.Country = entity.Country;
        existingEntity.Zip = entity.Zip;
        existingEntity.Phone1 = entity.Phone1;
        existingEntity.Phone2 = entity.Phone2;
        existingEntity.PrimaryUserId = entity.PrimaryUserId;
        existingEntity.SecondaryUserId = entity.SecondaryUserId;
        existingEntity.LastModifiedAt = DateTime.UtcNow;

        var result = await dbContext.SaveChangesAsync(cancellationToken);

        if (result <= 0)
        {
            logger.LogError("Failed to update address for organization {OrganizationId}.", entity.OrganizationId);
            throw new Exception("Failed to update organization address.");
        }
        else
        {
            logger.LogInformation("Address for organization {OrganizationId} updated successfully.", entity.OrganizationId);
        }

        return true;
    }

    public async Task<bool> DeleteAsync(OrgAddress entity, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(DeleteAsync));

        var existingEntity = await dbContext.OrgAddresses
            .FirstOrDefaultAsync(a => a.OrganizationId == entity.OrganizationId
                && a.RecordStatus == (int)RecordStatus.Active, cancellationToken);
        if (existingEntity == null)
        {
            logger.LogWarning("Address for organization {OrganizationId} not found, delete failed.", entity.OrganizationId);
            throw new KeyNotFoundException("Organization address not found, delete failed.");
        }

        existingEntity.RecordStatus = (int)RecordStatus.Inactive;
        existingEntity.LastModifiedAt = DateTime.UtcNow;

        var result = await dbContext.SaveChangesAsync(cancellationToken);

        if (result <= 0)
        {
            logger.LogError("Failed to delete address for organization {OrganizationId}.", entity.OrganizationId);
            throw new Exception("Failed to delete organization address.");
        }
        else
        {
            logger.LogInformation("Address for organization {OrganizationId} deleted successfully.", entity.OrganizationId);
        }

        return true;
    }

    public async Task<IReadOnlyList<OrgAddress>> GetAllAsync(CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(GetAllAsync));

        return await dbContext.OrgAddresses
            .AsNoTracking()
            .Where(a => a.RecordStatus == (int)RecordStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrgAddress>?> SearchByAsync(
        Expression<Func<OrgAddress, bool>> filter, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(SearchByAsync));

        return await dbContext.OrgAddresses
            .AsNoTracking()
            .Where(filter)
            .Where(a => a.RecordStatus == (int)RecordStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<OrgAddress?> GetByOrganizationIdAsync(OrganizationId organizationId, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(GetByOrganizationIdAsync));

        return await dbContext.OrgAddresses
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.OrganizationId == organizationId
                && a.RecordStatus == (int)RecordStatus.Active, cancellationToken);
    }

    private async Task EnsureContactsAreMembersAsync(OrgAddress entity, CancellationToken cancellationToken)
    {
        if (entity.PrimaryUserId is not null
            && !await IsActiveMemberAsync(entity.PrimaryUserId, entity.OrganizationId, cancellationToken))
        {
            logger.LogWarning("Primary contact {UserId} is not an active member of organization {OrganizationId}.",
                entity.PrimaryUserId, entity.OrganizationId);
            throw new InvalidOperationException("Primary contact must be an active member of the organization.");
        }

        if (entity.SecondaryUserId is not null
            && !await IsActiveMemberAsync(entity.SecondaryUserId, entity.OrganizationId, cancellationToken))
        {
            logger.LogWarning("Secondary contact {UserId} is not an active member of organization {OrganizationId}.",
                entity.SecondaryUserId, entity.OrganizationId);
            throw new InvalidOperationException("Secondary contact must be an active member of the organization.");
        }
    }

    private Task<bool> IsActiveMemberAsync(UserId userId, OrganizationId organizationId, CancellationToken cancellationToken) =>
        dbContext.AppUsers.AnyAsync(u => u.UserId == userId
            && u.RecordStatus == (int)RecordStatus.Active
            && (dbContext.UserDetails.Any(d => d.UserId == userId
                    && d.OrganizationId == organizationId
                    && d.RecordStatus == (int)RecordStatus.Active)
                || dbContext.OrgUserRoles.Any(r => r.UserId == userId
                    && r.OrganizationId == organizationId
                    && r.RecordStatus == (int)RecordStatus.Active)),
            cancellationToken);
}