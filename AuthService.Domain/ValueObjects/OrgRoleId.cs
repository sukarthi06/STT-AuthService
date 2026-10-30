namespace AuthService.Domain.ValueObjects;

public record OrgRoleId
{
    public Guid Value { get; }
    private OrgRoleId(Guid value) => Value = value;
    public static OrgRoleId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("OrgRoleId cannot be empty.", nameof(value));
        }
        return new OrgRoleId(value);
    }
    public static bool Empty(Guid value)
    {
        return value == Guid.Empty;
    }
}
