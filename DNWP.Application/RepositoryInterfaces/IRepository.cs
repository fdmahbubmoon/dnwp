using System.Linq.Expressions;

namespace DNWP.Application.RepositoryInterfaces;

public interface IRepository<TEntity> where TEntity : class
{

    Task<TEntity> InsertAsync(TEntity entity);

    Task InsertRangeAsync(List<TEntity> entities);

    Task<TEntity> UpdateAsync(TEntity entity);

    Task<bool> DeleteAsync(TEntity entity);

    Task<TEntity> FirstOrDefaultAsync(object id);
    
    Task<TEntity> FirstOrDefaultAsync(
       Expression<Func<TEntity, bool>> predicate,
       params Expression<Func<TEntity, object>>[] includes);

    Task<List<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
        params Expression<Func<TEntity, object>>[] includes
    );
}