namespace AuthService.Domain.Entities.Org;

public class Organization : Entity
{
    public OrganizationId OrganizationId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string LicenseNo { get; set; } = default!;
}
