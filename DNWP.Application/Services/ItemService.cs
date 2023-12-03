using DNWP.Application.Interfaces;
using DNWP.Common.Exceptions;
using DNWP.Domain.Entities;
using DNWP.Domain.Models;
using DNWP.Domain.ResponseModels;
using DNWP.Repository.Base;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;

namespace DNWP.Application.Services;

public class ItemService : BaseService<Item>, IItemService
{
    public ItemService(IRepository<Item> repository, IMemoryCache memoryCache, ICategoryService categoryService)
        : base(repository, nameof(Item), memoryCache)
    {
    }

    public async Task<List<ItemResponseModel>> GetAllAsync()
    {
        var items = await base.GetAllAsync(i => i.Category);
        return items.Select(s=> new ItemResponseModel
        {
            Id = s.Id,
            ItemName = s.ItemName,
            ItemUnit = s.ItemUnit,
            CategoryId = s.CategoryId,
            CategoryName = s.Category.CategoryName,
            ItemQuantity = s.ItemQuantity
        }).ToList();
    }

    public async Task<Item> AddAsyc(Item entity)
    {
        await ValidateItemAsync(entity);
        
        return await base.AddAsyc(entity);
    }

    public async Task<Item> UpdateAsync(long id, ItemDto item)
    {
        var entity = await base.GetByIdAsync(id);
        
        ArgumentNullException.ThrowIfNull(entity);

        entity.ItemName = item.ItemName;
        entity.CategoryId = item.CategoryId;
        entity.ItemQuantity = item.ItemQuantity;
        entity.ItemUnit = item.ItemUnit;
        await ValidateItemAsync(entity);

        await base.UpdateAsync(entity);
        return entity;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var entity = await base.GetByIdAsync(id);

        ArgumentNullException.ThrowIfNull(entity);

        return await base.DeleteAsync(entity);
    }

    public async Task<ItemResponseModel> FirstOrDefaultAsync(long id)
    {
        var item = await base.FirstOrDefaultAsync(item => item.Id == id, i => i.Category);
        return item.ToItemResponse();
    }

    public async Task AddBulkAsync()
    {

    }

    private async Task ValidateItemAsync(Item item)
    {
        var existingItem = await base
            .FirstOrDefaultAsync(c => c.ItemName == item.ItemName && c.Id != item.Id);

        if (existingItem is null)
            return;

        throw new ValidationException(nameof(item.ItemName));
    }
}
