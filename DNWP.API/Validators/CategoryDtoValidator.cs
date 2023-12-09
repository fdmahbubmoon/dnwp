using DNWP.Domain.Models;
using FluentValidation;

namespace DNWP.API.Validators;

public class CategoryDtoValidator : AbstractValidator<CategoryDto>
{
    public CategoryDtoValidator()
    {
        RuleFor(x => x.CategoryName).NotEmpty().WithMessage("Please specify a Category name");
    }
}
