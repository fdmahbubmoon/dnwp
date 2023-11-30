using DNWP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNWP.Domain.Models;

public record CategoryDto
{
    public required string CategoryName { get; set; }

    public Category ToCategory()
    {
        return new Category { CategoryName = CategoryName.Trim() };
    }
}
