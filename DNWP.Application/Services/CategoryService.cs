using DNWP.Application.Exceptions;
using DNWP.Application.Interfaces;
using DNWP.Domain.Entities;
using DNWP.Domain.Models;
using Microsoft.Extensions.Caching.Memory;
using OfficeOpenXml;
using DNWP.Application.RepositoryInterfaces;

namespace DNWP.Application.Services;

public class CategoryService: BaseServiceFactory<Category>, ICategoryService
{
    public CategoryService(IRepository<Category> repository, IMemoryCache memoryCache)
        : base(repository, nameof(Category), memoryCache)
    {
    }

    public async Task<Category> AddAsync(Category entity)
    {
        await ValidateCategoryAsync(entity);
        
        return await base.AddAsync(entity);
    }

    public async Task<Category> UpdateAsync(long id, CategoryDto category)
    {
        var entity = await base.GetByIdAsync(id);
        
        ArgumentNullException.ThrowIfNull(entity);

        entity.CategoryName = category.CategoryName;
        await ValidateCategoryAsync(entity);

        await base.UpdateAsync(entity);
        return entity;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var entity = await base.GetByIdAsync(id);

        ArgumentNullException.ThrowIfNull(entity);

        return await base.DeleteAsync(entity);
    }

    private List<CategoryDto> GetCategoriesFromStream(Stream stream)
    {
        var categories = new List<CategoryDto>();
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
                        categories.Add(new CategoryDto
                        {
                            CategoryName = workSheet.GetValue(i, 1)?.ToString()
                        });
                    }
                }
            }
        }

        return categories;
    }

    public async Task<string> AddBulkAsync(Stream stream)
    {
        var categories = GetCategoriesFromStream(stream);
        var validCategories = await ValidateImportedDataAsync(categories);

        if (validCategories.Any())
        {
            await base.AddRangeAsync(validCategories);
        }
        return $"Total: {categories.Count}. " +
            $"Valid: {validCategories.Count}. " +
            $"Invalid: {categories.Count - validCategories.Count}";
    }

    private async Task ValidateCategoryAsync(Category category)
    {
        var existingCategory = await base
            .FirstOrDefaultAsync(c => c.CategoryName == category.CategoryName && c.Id != category.Id);

        if (existingCategory is null)
            return;

        throw new ValidationException("Category Name");
    }
    private async Task<List<Category>> ValidateImportedDataAsync(List<CategoryDto> categories)
    {
        List<Category> validCategories = new();

        var existingCategories = await GetAllAsync(s => s.OrderBy(o => o.Id));

        foreach(CategoryDto category in categories)
        {
            if (existingCategories.Exists(c => c.CategoryName == category.CategoryName))
                continue;

            if (validCategories.Exists(c => c.CategoryName == category.CategoryName))
                continue;

            validCategories.Add(category.ToCategory());
        }

        return validCategories;
    }
}
