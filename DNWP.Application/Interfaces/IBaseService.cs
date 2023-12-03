using System.Linq.Expressions;

namespace DNWP.Application.Interfaces;

public interface IBaseService<TEntity> where TEntity : class
{
    Task<List<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes);
    Task<TEntity> GetByIdAsync(long id);
    Task<TEntity> AddAsyc(TEntity entity);
    Task<List<TEntity>> AddRangeAsync(List<TEntity> entities);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task<bool> DeleteAsync(TEntity entity);
}
