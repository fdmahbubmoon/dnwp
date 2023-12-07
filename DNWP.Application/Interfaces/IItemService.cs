using DNWP.Domain.Entities;
using DNWP.Domain.Models;
using DNWP.Domain.ResponseModels;

namespace DNWP.Application.Interfaces;

public interface IItemService: IBaseServiceFactory<Item>
{
    Task<Item> UpdateAsync(long id, ItemDto item);
    Task<bool> DeleteAsync(long id);
    Task<ItemResponseModel> FirstOrDefaultAsync(long id);
    Task<List<ItemResponseModel>> GetAllAsync();
    Task<string> AddBulkAsync(Stream stream);
    Task<PagedList<ItemResponseModel>> GetPageAsync(int index, int size);

}
