namespace AuthService.Domain.ValueObjects;

public record DepartmentId
{
    public Guid Value { get; }
    private DepartmentId(Guid value) => Value = value;
    public static DepartmentId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("DepartmentId cannot be empty.", nameof(value));
        }
        return new DepartmentId(value);
    }
    public static bool Empty(Guid value)
    {
        return value == Guid.Empty;
    }
}
