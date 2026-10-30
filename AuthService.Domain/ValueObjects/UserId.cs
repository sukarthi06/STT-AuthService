namespace AuthService.Domain.ValueObjects;

public record UserId
{
    public Guid Value { get; }
    private UserId(Guid value) => Value = value;
    public static UserId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("UserId cannot be empty.", nameof(value));
        }
        return new UserId(value);
    }
    public static bool Empty(Guid value)
    {
        return value == Guid.Empty;
    }
}
