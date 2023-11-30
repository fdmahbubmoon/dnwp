namespace DNWP.Domain.Entities;
public class Item
{
    public required long Id { get; set; }
    public required string ItemName { get; set; }
    public required string ItemUnit { get; set; }
    public decimal ItemQuantity { get; set; }
    public long CategoryId { get; set; }
    public Category Category { get; set; }
}
