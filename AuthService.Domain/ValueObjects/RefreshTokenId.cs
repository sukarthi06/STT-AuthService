namespace AuthService.Domain.ValueObjects;

public record RefreshTokenId
{
    public Guid Value { get; }
    private RefreshTokenId(Guid value) => Value = value;
    public static RefreshTokenId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("RefreshTokenId cannot be empty.", nameof(value));
        }
        return new RefreshTokenId(value);
    }
    public static bool Empty(Guid value)
    {
        return value == Guid.Empty;
    }
}