using System.Collections.Concurrent;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SchedulerService.Database;

namespace SchedulerService.Repositories;

public class RepositoryBase<TEntity> : ICrudRepository<TEntity> where TEntity : class
{
    private static readonly ConcurrentDictionary<object, Expression<Func<TEntity, bool>>> CachedLambdas = new();
    private readonly SchedulerContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public RepositoryBase(SchedulerContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    /// <inheritdoc/>
    public virtual async Task CreateAsync(TEntity entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
    }
    
    /// <inheritdoc/>
    public virtual async Task CreateRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        await _dbSet.AddRangeAsync(entities, ct);
    }

    /// <inheritdoc/>
    public virtual async Task<TEntity?> GetByIdAsync(
        object id,
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, object?>>[]? includeProperties = null,
        CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (includeProperties is not null)
            foreach (var property in includeProperties)
                query = query.Include(property);

        if (predicate is not null)
            query = query.Where(predicate);
        
        var byIdLambda = CachedLambdas.GetOrAdd(id, _ =>
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var left = Expression.Property(parameter, "Id");
            var right = Expression.Constant(id);
            var equal = Expression.Equal(left, right);
            return Expression.Lambda<Func<TEntity, bool>>(equal, parameter);
        });
        
        return await query.FirstOrDefaultAsync(byIdLambda, cancellationToken: ct);
    }
    
    /// <inheritdoc/>
    public async Task<TEntity?> FirstOrDefaultAsync(object id,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true,
        CancellationToken ct = default)
    {
        IQueryable<TEntity> query = _dbSet;
        
        if (disableTracking)
            query = query.AsNoTracking();

        if (include is not null)
            query = include(query);

        if (predicate is not null)
            query = query.Where(predicate);

        var byIdLambda = CachedLambdas.GetOrAdd(id, _ =>
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var left = Expression.Property(parameter, "Id");
            var right = Expression.Constant(id);
            var equal = Expression.Equal(left, right);
            return Expression.Lambda<Func<TEntity, bool>>(equal, parameter);
        });

        return await query.FirstOrDefaultAsync(byIdLambda, ct);
    }
    
    /// <inheritdoc/>
    public virtual async Task<IEnumerable<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, object?>>[]? includeProperties = null,
        CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (predicate is not null)
            query = query.Where(predicate);

        if (includeProperties is not null)
            foreach (var property in includeProperties)
                query = query.Include(property);

        return await query.ToListAsync(ct);
    }

    /// <inheritdoc/>
    public virtual async Task<bool> UpdateAsync(object id, TEntity entity, CancellationToken ct = default)
    {
        var dbEntity = await _dbSet.FindAsync(new[] { id }, cancellationToken: ct);
        
        if (dbEntity is null)
            return false;

        _context.Entry(dbEntity).CurrentValues.SetValues(entity);
        return true;
    }

    /// <inheritdoc/>
    public virtual async Task<bool> DeleteByIdAsync(object id, CancellationToken ct = default)
    {
        var entity = await _dbSet.FindAsync(new[] { id }, cancellationToken: ct);

        if (entity is null)
            return false;

        await this.DeleteAsync(entity, ct);
        return true;
    }

    /// <inheritdoc/>
    public virtual Task DeleteAsync(TEntity entity, CancellationToken ct = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        _context.RemoveRange(entities);
        return Task.CompletedTask;
    }
    
    /// <inheritdoc/>
    public async Task SaveAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}