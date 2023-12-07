using DNWP.Application.Interfaces;
using DNWP.Domain.Models;
using DNWP.Infrastructure;
using DNWP.Repository.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;

namespace DNWP.Application.Services;

public class BaseServiceFactory<TEntity> : IBaseServiceFactory<TEntity> where TEntity : class
{
    private readonly IRepository<TEntity> _repository;
    private readonly string _cacheKey;
    private readonly IMemoryCache _memoryCache;
    public BaseServiceFactory(IRepository<TEntity> repository, string cacheKey, IMemoryCache memoryCache)
    {
        _repository = repository;
        _cacheKey = cacheKey;
        _memoryCache = memoryCache;
    }
    public virtual async Task<TEntity> AddAsyc(TEntity entity)
    {
        await _repository.InsertAsync(entity);
        _memoryCache.Remove(_cacheKey);
        return entity;
    }

    public virtual async Task<List<TEntity>> AddRangeAsync(List<TEntity> entities)
    {
        await _repository.InsertRangeAsync(entities);
        _memoryCache.Remove(_cacheKey);
        return entities;
    }

    public virtual async Task<bool> DeleteAsync(TEntity entity)
    {
        var result = await _repository.DeleteAsync(entity);

        if (!result)
            return false;

        _memoryCache.Remove(_cacheKey);

        return true;
    }

    public virtual async Task<List<TEntity>> GetAllAsync(
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
        params Expression<Func<TEntity, object>>[] includes)
    {
        var cachedData = _memoryCache.Get(_cacheKey) as List<TEntity>;

        if (cachedData is not null)
            return cachedData;

        var entities = await _repository.GetAllAsync(a=> true, orderBy, includes);

        _memoryCache.Set(_cacheKey, entities);

        return entities;
    }

    public virtual async Task<PagedList<TEntity>> GetPageAsync(int index, 
        int size, 
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, 
        params Expression<Func<TEntity, object>>[] includes)
    {
        string pageCacheKey = _cacheKey;
        var cachedData = _memoryCache.Get(pageCacheKey) as List<TEntity>;

        if (cachedData is not null)
        {
            return new PagedList<TEntity>()
            {
                TotalPages = Convert.ToInt32(Math.Ceiling(1.0 * cachedData.Count / size)),
                TotalCount = cachedData.Count,
                Data = cachedData.Skip(index * size).Take(size).ToList()
            };
        }

        var entities = await _repository.GetAllAsync(a => true, orderBy,includes);
        _memoryCache.Set(pageCacheKey, entities);

        var totalCount = entities.Count;

        return new PagedList<TEntity>()
        {
            TotalPages = Convert.ToInt32(Math.Ceiling(1.0 * totalCount / size)),
            TotalCount = totalCount,
            Data = entities.Skip(index * size).Take(size).ToList()
        };
    }

    public virtual async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, 
        params Expression<Func<TEntity, object>>[] includes)
    {
        return await _repository.FirstOrDefaultAsync(predicate, includes);
    }

    public virtual async Task<TEntity> GetByIdAsync(long id)
    {
        return await _repository.FirstOrDefaultAsync(id);
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        await _repository.UpdateAsync(entity);
        _memoryCache.Remove(_cacheKey);
        return entity;
    }
}
