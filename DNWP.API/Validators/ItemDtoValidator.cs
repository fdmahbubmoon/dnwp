using DNWP.Domain.Models;
using FluentValidation;

namespace DNWP.API.Validators;

public class ItemDtoValidator : AbstractValidator<ItemDto>
{
    public ItemDtoValidator()
    {
        RuleFor(x => x.ItemName).NotEmpty().WithMessage("Please specify an Item name");
        RuleFor(x => x.ItemQuantity).NotNull().NotEmpty().WithMessage("Item(s) must have a quantity");
        RuleFor(x => x.ItemUnit).NotNull().NotEmpty().WithMessage("Unit of quantity is required");
        RuleFor(x => x.CategoryId).NotNull().NotEmpty().WithMessage("Please specify a category for Item");
    }

}
