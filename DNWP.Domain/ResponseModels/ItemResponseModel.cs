using DNWP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNWP.Domain.ResponseModels;

public class ItemResponseModel
{
    public long Id { get; set; }
    public required string ItemName { get; set; }
    public required string ItemUnit { get; set; }
    public decimal ItemQuantity { get; set; }
    public long CategoryId { get; set; }
    public string? CategoryName { get; set; }
}
