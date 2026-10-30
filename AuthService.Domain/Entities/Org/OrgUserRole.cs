using AuthService.Domain.Entities.Users;

namespace AuthService.Domain.Entities.Org;

public class OrgUserRole : Entity
{
    public OrganizationId OrganizationId { get; set; } = default!;
    public UserId UserId { get; set; } = default!;
    public DepartmentId? DepartmentId { get; set; }
    public OrgRoleId? OrgRoleId { get; set; }
    public bool AllDepartments { get; set; }
    public bool AllRoles { get; set; }
    public AppUser User { get; set; } = default!;
    public OrganizationDept? Department { get; set; }
    public OrganizationRole? Role { get; set; }
}
