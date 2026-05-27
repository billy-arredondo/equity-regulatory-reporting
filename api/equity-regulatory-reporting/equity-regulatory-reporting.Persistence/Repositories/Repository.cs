using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Persistence.Repositories;

public class Repository<T>(ApplicationDbContext context) : IRepository<T> where T : AuditableEntity
{
    private readonly DbSet<T> _set = context.Set<T>();

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _set.FindAsync([id], cancellationToken);

    public async Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default)
        => await _set.ToListAsync(cancellationToken);

    public IQueryable<T> Query() => _set.AsQueryable();

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _set.AddAsync(entity, cancellationToken);

    public void Update(T entity) => _set.Update(entity);

    public void Remove(T entity) => _set.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
