namespace AuthService.Domain.Entities.Users;

public class AppUserRole : Entity
{
    public UserId UserId { get; set; } = default!;
    public RoleID RoleId { get; set; } = default!;
    public AppRole Role { get; set; } = default!;
}
