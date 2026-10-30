namespace AuthService.Domain.ValueObjects;

public record RoleID
{
    public Guid Value { get; }
    private RoleID(Guid value) => Value = value;
    public static RoleID Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("RoleID cannot be empty.", nameof(value));
        }
        return new RoleID(value);
    }
    public static bool Empty(Guid value)
    {
        return value == Guid.Empty;
    }
}
