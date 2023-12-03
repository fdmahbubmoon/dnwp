using System.Linq.Expressions;

namespace DNWP.Repository.Base;

public interface IRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> Query();
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate = null);

    Task<TEntity> InsertAsync(TEntity entity);

    Task InsertRangeAsync(List<TEntity> entities);

    Task<TEntity> UpdateAsync(TEntity entity);

    Task UpdateRangeAsync(List<TEntity> entities);

    Task DeleteRangeAsync(List<TEntity> entities);

    Task<bool> DeleteAsync(TEntity entity);

    Task<TEntity> FirstOrDefaultAsync(object id);
    Task<TEntity> GetEntityAsNoTrackingAsync(object id);

    Task<TEntity> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate
    );

    Task<TEntity> FirstOrDefaultAsync(
       Expression<Func<TEntity, bool>> predicate,
       params Expression<Func<TEntity, object>>[] includes
   );

    Task<TEntity> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy
    );

    Task<TEntity> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
        params Expression<Func<TEntity, object>>[] includes
    );

    Task<List<TEntity>> GetAllAsync();

    Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);

    Task<List<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> predicate,
        params Expression<Func<TEntity, object>>[] include
    );

    Task<List<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes);

    Task<List<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy
    );

    Task<List<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
        params Expression<Func<TEntity, object>>[] includes
    );

    void Attach(TEntity entity);
    void Detach(TEntity entity);

}