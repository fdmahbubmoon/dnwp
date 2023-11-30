using DNWP.Application.Interfaces;
using DNWP.Domain.Entities;
using DNWP.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace DNWP.API.Controllers;
[Route("api/[controller]")]
[ApiController]
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
    public async Task<IActionResult> AddAsync([FromBody] CategoryDto categoryModel)
    {
        var category = categoryModel.ToCategory();
        await _categoryService.AddAsyc(category);
        return Ok(category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(long id, [FromBody] CategoryDto category)
    {

        await _categoryService.UpdateAsync(id, category);
        return Ok(category);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        var isDeleted = await _categoryService.DeleteAsync(id);
        return Ok(isDeleted);
    }
}
