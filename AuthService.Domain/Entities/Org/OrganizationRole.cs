namespace AuthService.Domain.Entities.Org;

public class OrganizationRole : Entity
{
    public OrgRoleId RoleId { get; set; } = default!;
    public OrganizationId OrganizationId { get; set; } = default!;
    public string RoleName { get; set; } = default!;
}
