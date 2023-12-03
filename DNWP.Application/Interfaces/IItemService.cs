using DNWP.Domain.Entities;
using DNWP.Domain.Models;
using DNWP.Domain.ResponseModels;

namespace DNWP.Application.Interfaces;

public interface IItemService: IBaseService<Item>
{
    Task<Item> UpdateAsync(long id, ItemDto item);
    Task<bool> DeleteAsync(long id);
    Task<ItemResponseModel> FirstOrDefaultAsync(long id);
    Task<List<ItemResponseModel>> GetAllAsync();

}
