using DNWP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNWP.Domain.Models;

public record ItemDto
{
    public required string ItemName { get; set; }
    public required string ItemUnit { get; set; }
    public decimal ItemQuantity { get; set; }
    public long CategoryId { get; set; }
    public Item ToItem()
    {
        return new Item { 
            ItemName = ItemName,
            ItemQuantity = ItemQuantity,
            ItemUnit = ItemUnit,
            CategoryId = CategoryId
        };
    }
}

public record ItemBulkDto
{
    public required string ItemName { get; set; }
    public required string ItemUnit { get; set; }
    public decimal ItemQuantity { get; set; }
    public string CategoryName { get; set; }
}
