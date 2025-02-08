using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace SchedulerService.Repositories;

public interface ICrudRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Creates an entity
    /// </summary>
    /// <param name="entity">New entity</param>
    /// <param name="ct">Cancellation Token</param>
    Task CreateAsync(TEntity entity, CancellationToken ct = default);

    /// <summary>
    /// Creates range of entities
    /// </summary>
    /// <param name="entities">Range of new entities</param>
    /// <param name="ct">Cancellation Token</param>
    Task CreateRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

    /// <summary>
    /// Gets entity by it's id from database
    /// </summary>
    /// <param name="id">Entity's id</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="includeProperties">A lambda expression representing the navigation property to be included (e => e.Property1).</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>Returns entity if it exists, if not returns null</returns>
    Task<TEntity?> GetByIdAsync(
        object id,
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, object?>>[]? includeProperties = null,
        CancellationToken ct = default);

    /// <summary>
    /// Gets the first or default entity based on a predicate, order by delegate and include delegate. This method is by default tracking query.
    /// </summary>
    /// <param name="id">Entity's id.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="orderBy">A function to order elements.</param>
    /// <param name="include">A function to include navigation properties</param>
    /// <param name="disableTracking"><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>An entity that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
    /// <remarks>This method is by default tracking query.</remarks>
    public Task<TEntity?> FirstOrDefaultAsync(object id,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = false, CancellationToken ct = default);
    
    /// <summary>
    /// Gets list of entities from database
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="includeProperties">A lambda expression representing the navigation property to be included (e => e.Property1).</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>List of entities</returns>
    Task<IEnumerable<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, object?>>[]? includeProperties = null,
        CancellationToken ct = default);
    
    /// <summary>
    /// Updates entity by it's id in database
    /// </summary>
    /// <param name="id">Entity's id</param>
    /// <param name="entity">Updated entity</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>Returns true if it was updated, or false if it was not found</returns>
    Task<bool> UpdateAsync(object id, TEntity entity, CancellationToken ct = default);
    
    /// <summary>
    /// Deletes entity by it's id in database
    /// </summary>
    /// <param name="id">Entity's id</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>Returns true if it was deleted, or false if it was not found</returns>
    Task<bool> DeleteByIdAsync(object id, CancellationToken ct = default);
    
    /// <summary>
    /// Deletes range of entities in database
    /// </summary>
    /// <param name="entities">Range of entities to delete</param>
    /// <param name="ct">Cancellation Token</param>
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);
    
    /// <summary>
    /// Deletes entity in database
    /// </summary>
    /// <param name="entity">Entity to delete</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>Returns true if it was deleted, or false if it was not found</returns>
    Task DeleteAsync(TEntity entity, CancellationToken ct = default);

    /// <summary>
    /// Saves changes in database
    /// </summary>
    /// <param name="ct">Cancellation Token</param>
    Task SaveAsync(CancellationToken ct = default);
}