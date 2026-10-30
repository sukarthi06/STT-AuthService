namespace AuthService.Domain.Entities.Org;

public class OrganizationDept : Entity
{
    public DepartmentId DeptId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public OrganizationId OrganizationId { get; set; } = default!;
}
