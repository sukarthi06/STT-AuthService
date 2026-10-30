namespace AuthService.Domain.Entities.Org;

public class OrgAddress : Entity
{
    public OrganizationId OrganizationId { get; set; } = default!;
    public string AddressLine1 { get; set; } = default!;
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? Zip { get; set; }
    public string? Phone1 { get; set; }
    public string? Phone2 { get; set; }
    public UserId? PrimaryUserId { get; set; }
    public UserId? SecondaryUserId { get; set; }

}
