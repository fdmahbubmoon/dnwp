using DNWP.Application.Interfaces;
using DNWP.Domain.Models;
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
        var categories = await _categoryService.GetAllAsync();
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
        var category = categoryModel.ToCategory();
        await _categoryService.AddAsyc(category);
        return Ok(category);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PutAsync(long id, [FromBody] CategoryDto category)
    {
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
}
