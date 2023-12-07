using DNWP.Domain.Models;
using System.Linq.Expressions;

namespace DNWP.Application.Interfaces;

public interface IBaseServiceFactory<TEntity> where TEntity : class
{
    Task<List<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, 
        params Expression<Func<TEntity, object>>[] includes);
    Task<TEntity> GetByIdAsync(long id);
    Task<TEntity> AddAsyc(TEntity entity);
    Task<List<TEntity>> AddRangeAsync(List<TEntity> entities);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task<bool> DeleteAsync(TEntity entity);
    Task<PagedList<TEntity>> GetPageAsync(int index, 
        int size,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
        params Expression<Func<TEntity, object>>[] includes);
}
