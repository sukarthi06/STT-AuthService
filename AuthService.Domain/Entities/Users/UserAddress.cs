namespace AuthService.Domain.Entities.Users;

public class UserAddress : Entity
{
    public int AddressId { get; set; } = default!;
    public UserId UserId { get; set; } = default!;
    public string AddressLine1 { get; set; } = default!;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = default!;
    public string State { get; set; } = default!;
    public string Country { get; set; } = default!;
    public string Zip { get; set; } = default!;
}
