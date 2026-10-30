namespace AuthService.Application.Repos.Org;

public interface IOrgAddressRepo : IRepo<OrgAddress>
{
    Task<OrgAddress?> GetByOrganizationIdAsync(OrganizationId organizationId, CancellationToken cancellationToken);
}