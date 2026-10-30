using AuthService.Domain.Entities.Org;
using AuthService.Domain.Enums;

namespace AuthService.Domain.Entities.Users;

public class UserDetails : Entity
{
    public UserId UserId { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? MiddleName { get; set; }
    public DateTime? DOB { get; set; }
    public string? Phone1 { get; set; }
    public string? Phone2 { get; set; }
    public UserSex? Sex { get; set; }
    public string? LicenseNo { get; set; }
    public OrganizationId? OrganizationId { get; set; }
    public Organization? Organization { get; set; }
}
