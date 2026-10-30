using System.Linq.Expressions;

namespace AuthService.Application.Abstraction;

public interface IRepo<T> where T : class
{
    Task<T> CreateAsync(T entity, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<T>?> SearchByAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken);
}
