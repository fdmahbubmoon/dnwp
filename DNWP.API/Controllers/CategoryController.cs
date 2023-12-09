using DNWP.API.Validators;
using DNWP.Application.Interfaces;
using DNWP.Domain.Entities;
using DNWP.Domain.Models;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DNWP.API.Controllers;

[Authorize(Roles = "Admin,General")]
[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var categories = await _categoryService.GetAllAsync(s => s.OrderBy(o => o.Id));
        return Ok(categories);
    }

    [HttpGet("page")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPageAsync([FromQuery ]int index, [FromQuery] int size)
    {
        PagedList<Category> categories = await _categoryService.GetPageAsync(index, size, s => s.OrderBy(o => o.Id));
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(long id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        return Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddAsync([FromBody] CategoryDto categoryModel)
    {
        Validate(categoryModel);
        var category = categoryModel.ToCategory();
        await _categoryService.AddAsync(category);
        return Ok(category);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateAsync(long id, [FromBody] CategoryDto category)
    {
        Validate(category);
        await _categoryService.UpdateAsync(id, category);
        return Ok(category);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        var isDeleted = await _categoryService.DeleteAsync(id);
        return Ok(isDeleted);
    }

    [HttpPost("bulk")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UploadBulkAsync()
    {
        var file = Request.Form.Files[0];
        if (file == null || file.Length == 0)
            return Content("File Not Selected!");

        string fileExtension = Path.GetExtension(file.FileName);
        if (fileExtension != ".xls" && fileExtension != ".xlsx")
            return Content("File is not in Excel Format!");

        var inputstream = file.OpenReadStream();
        var response = await _categoryService.AddBulkAsync(inputstream);
        return Ok(new { Message = response });
    }

    private static void Validate(CategoryDto category)
    {
        CategoryDtoValidator validator = new();

        ValidationResult result = validator.Validate(category);

        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
    }
}
