namespace AuthService.Domain.ValueObjects;

public record OrganizationId
{
    public Guid Value { get; }
    private OrganizationId(Guid value) => Value = value;
    public static OrganizationId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("OrganizationId cannot be empty.", nameof(value));
        }
        return new OrganizationId(value);
    }
    public static bool Empty(Guid value)
    {
        return value == Guid.Empty;
    }
}
