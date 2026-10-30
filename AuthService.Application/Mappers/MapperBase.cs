namespace AuthService.Application.Mappers;

public abstract class MapperBase
{
    protected Guid ParseGuid(string id)
    {
        if (!Guid.TryParse(id, out var guid))
            throw new ArgumentException($"'{id}' is not a valid id.", nameof(id));
        return guid;
    }
}

