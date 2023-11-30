using DNWP.Application.Interfaces;
using DNWP.Common.Exceptions;
using DNWP.Domain.Entities;
using DNWP.Domain.Models;
using DNWP.Repository.Base;
using Microsoft.Extensions.Caching.Memory;

namespace DNWP.Application.Services;

public class CategoryService: BaseService<Category>, ICategoryService
{
    private readonly IMemoryCache _memoryCache;
    public CategoryService(IRepository<Category> repository, IMemoryCache memoryCache)
        : base(repository, nameof(Category), memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task<Category> AddAsyc(Category entity)
    {
        await ValidateCategoryAsync(entity);
        
        return await base.AddAsyc(entity);
    }

    public async Task<Category> UpdateAsync(long id, CategoryDto category)
    {
        var entity = await base.GetByIdAsync(id);
        
        ArgumentNullException.ThrowIfNull(entity);

        entity.CategoryName = category.CategoryName;
        await ValidateCategoryAsync(entity);

        await base.UpdateAsync(entity);

        var cachedData = _memoryCache.Get(nameof(Category)) as List<Category> ?? new List<Category>();
        var cacheToBeUpdated = cachedData.FirstOrDefault(cache => cache.Id == id)!;
        cacheToBeUpdated.CategoryName = entity.CategoryName;
        _memoryCache.Set(nameof(Category), cachedData);

        return entity;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var entity = await base.GetByIdAsync(id);

        ArgumentNullException.ThrowIfNull(entity);

        return await base.DeleteAsync(entity);
    }

    public async Task AddBulkAsync()
    {

    }

    private async Task ValidateCategoryAsync(Category category)
    {
        var existingCategory = await base
            .FirstOrDefaultAsync(c => c.CategoryName == category.CategoryName && c.Id != category.Id);

        if (existingCategory is null)
            return;

        throw new ValidationException(nameof(category.CategoryName));
    }
}
