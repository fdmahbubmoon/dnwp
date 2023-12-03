using DNWP.Domain.Entities;
using DNWP.Domain.Models;

namespace DNWP.Application.Interfaces;

public interface ICategoryService : IBaseService<Category>
{
    Task<Category> UpdateAsync(long id, CategoryDto category);
    Task<bool> DeleteAsync(long id);
    Task<string> AddBulkAsync(Stream stream);
}
