using AuthService.Domain.Enums;

namespace AuthService.Domain.Entities.Users;

public class AppRole : Entity
{
    public RoleID RoleID { get; set; } = default!;
    public AppRoles RoleName { get; set; } = default!;
}