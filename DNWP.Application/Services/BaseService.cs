using DNWP.Application.Interfaces;
using DNWP.Infrastructure;
using DNWP.Repository.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;

namespace DNWP.Application.Services;

public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class
{
    private readonly IRepository<TEntity> _repository;
    private readonly string _cacheKey;
    private readonly IMemoryCache _memoryCache;
    public BaseService(IRepository<TEntity> repository, string cacheKey, IMemoryCache memoryCache)
    {
        _repository = repository;
        _cacheKey = cacheKey;
        _memoryCache = memoryCache;
    }
    public virtual async Task<TEntity> AddAsyc(TEntity entity)
    {
        await _repository.InsertAsync(entity);

        var cachedData = _memoryCache.Get(_cacheKey) as List<TEntity> ?? new List<TEntity>();
        cachedData.Add(entity);
        _memoryCache.Set(_cacheKey, cachedData);

        return entity;
    }

    public virtual async Task<bool> DeleteAsync(TEntity entity)
    {
        var result = await _repository.DeleteAsync(entity);

        if (!result)
            return false;

        var cachedData = _memoryCache.Get(_cacheKey) as List<TEntity> ?? new List<TEntity>();
        cachedData.Remove(entity);
        _memoryCache.Set(_cacheKey, cachedData);

        return true;
    }

    public virtual async Task<List<TEntity>> GetAllAsync()
    {
        var cachedData = _memoryCache.Get(_cacheKey) as List<TEntity>;

        if (cachedData is not null)
            return cachedData;

        var entities = await _repository.GetAllAsync();

        _memoryCache.Set(_cacheKey, entities);

        return entities;
    }

    public virtual async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _repository.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<TEntity> GetByIdAsync(long id)
    {
        return await _repository.FirstOrDefaultAsync(id);
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        return await _repository.UpdateAsync(entity);
    }
}
