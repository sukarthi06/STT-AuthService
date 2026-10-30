using AuthService.Domain.Entities.Org;
using AuthService.Domain.Enums;

namespace AuthService.Domain.Entities.Users;

public class AppUser : Entity
{
    public UserId UserId { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Pwd { get; set; } = default!;
    public UserStatus UserStatus { get; set; } = UserStatus.Active;    
}
