using DNWP.Domain.ResponseModels;

namespace DNWP.Domain.Entities;
public class Item
{
    public long Id { get; set; }
    public required string ItemName { get; set; }
    public required string ItemUnit { get; set; }
    public decimal ItemQuantity { get; set; }
    public long CategoryId { get; set; }
    public Category Category { get; set; }

    public ItemResponseModel ToItemResponse()
    {
        return new ItemResponseModel
        {
            ItemName = ItemName,
            ItemQuantity = ItemQuantity,
            ItemUnit = ItemUnit,
            CategoryId = CategoryId,
            CategoryName = Category?.CategoryName
        };
    }
}
