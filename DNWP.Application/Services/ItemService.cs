using DNWP.Application.Exceptions;
using DNWP.Application.Interfaces;
using DNWP.Domain.Entities;
using DNWP.Domain.Models;
using DNWP.Domain.ResponseModels;
using Microsoft.Extensions.Caching.Memory;
using OfficeOpenXml;
using DNWP.Application.RepositoryInterfaces;

namespace DNWP.Application.Services;

public class ItemService : BaseServiceFactory<Item>, IItemService
{
    private readonly ICategoryService _categoryService;
    public ItemService(IRepository<Item> repository, IMemoryCache memoryCache, ICategoryService categoryService)
        : base(repository, nameof(Item), memoryCache)
    {
        _categoryService = categoryService;
    }

    public async Task<PagedList<ItemResponseModel>> GetPageAsync(int index, int size)
    {
        var pagedItems = await base.GetPageAsync(index, size, s=> s.OrderBy(o=>o.Id), i => i.Category);

        var pagedItemResponse = new PagedList<ItemResponseModel>
        {
            PageSize = pagedItems.PageSize,
            TotalCount = pagedItems.TotalCount,
            TotalPages = pagedItems.TotalPages
        };

        pagedItemResponse.Data = pagedItems.Data.Select(s => new ItemResponseModel
        {
            Id = s.Id,
            ItemName = s.ItemName,
            ItemUnit = s.ItemUnit,
            CategoryId = s.CategoryId,
            CategoryName = s.Category.CategoryName,
            ItemQuantity = s.ItemQuantity
        }).ToList();

        return pagedItemResponse;
    }

    public async Task<List<ItemResponseModel>> GetAllAsync()
    {
        var items = await base.GetAllAsync(o => o.OrderBy(a=>a.Id),i => i.Category);
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

    public async Task<Item> AddAsync(Item entity)
    {
        await ValidateItemAsync(entity);
        
        return await base.AddAsync(entity);
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

    private async Task ValidateItemAsync(Item item)
    {
        var existingItem = await base
            .FirstOrDefaultAsync(c => c.ItemName == item.ItemName && c.Id != item.Id);

        if (existingItem is null)
            return;

        throw new ValidationException(nameof(item.ItemName));
    }

    public async Task<string> AddBulkAsync(Stream stream)
    {
        var items = GetItemsFromStream(stream);
        var validItems = await ValidateImportedDataAsync(items);

        if (validItems.Any())
        {
            await base.AddRangeAsync(validItems);
        }
        return $"Total: {items.Count}. " +
            $"Valid: {validItems.Count}. " +
            $"Invalid: {items.Count - validItems.Count}";
    }

    private List<ItemBulkDto> GetItemsFromStream(Stream stream)
    {
        var items = new List<ItemBulkDto>();
        using (ExcelPackage package = new ExcelPackage(stream))
        {
            ExcelWorkbook workBook = package.Workbook;
            if (workBook is not null)
            {
                ExcelWorksheet workSheet = workBook.Worksheets.FirstOrDefault();
                if (workSheet is not null)
                {
                    int totalRows = workSheet.Dimension.Rows;
                    for (int i = 2; i <= totalRows; i++)
                    {
                        items.Add(new ItemBulkDto
                        {
                            ItemName = workSheet.GetValue(i, 1)?.ToString(),
                            ItemUnit = workSheet.GetValue(i, 2)?.ToString(),
                            ItemQuantity = Convert.ToDecimal(workSheet.GetValue(i, 3)),
                            CategoryName = workSheet.GetValue(i, 4)?.ToString()
                        });
                    }
                }
            }
        }

        return items;
    }

    private async Task<List<Item>> ValidateImportedDataAsync(List<ItemBulkDto> items)
    {
        List<Item> validItems = new();

        var existingItems = await GetAllAsync();

        var existingCategories = await _categoryService.GetAllAsync(s => s.OrderBy(o => o.Id));

        foreach (ItemBulkDto item in items)
        {
            var category = existingCategories.FirstOrDefault(c => c.CategoryName == item.CategoryName);
            
            if (existingItems.Exists(i => i.ItemName == item.ItemName) || category is null)
                continue;

            if (validItems.Exists(c => c.ItemName == item.ItemName))
                continue;

            var validItem = item.ToItem();
            validItem.CategoryId = category!.Id;

            validItems.Add(validItem);
        }

        return validItems;
    }
}
