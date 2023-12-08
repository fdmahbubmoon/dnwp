using DNWP.Application.Interfaces;
using DNWP.Application.Services;
using DNWP.Domain.Entities;
using DNWP.Domain.Models;
using DNWP.Domain.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DNWP.API.Controllers;

[Authorize(Roles = "Admin,General")]
[ApiController]
[Route("api/[controller]")]
public class ItemController : ControllerBase
{
    private readonly IItemService _itemService;
    public ItemController(IItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpGet("page")]
    public async Task<IActionResult> GetPageAsync([FromQuery] int index, [FromQuery] int size)
    {
        PagedList<ItemResponseModel> categories = await _itemService.GetPageAsync(index, size);
        return Ok(categories);
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var items = await _itemService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(long id)
    {
        var item = await _itemService.FirstOrDefaultAsync(id);
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] ItemDto itemModel)
    {
        var item = itemModel.ToItem();
        await _itemService.AddAsync(item);
        return Ok(item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(long id, [FromBody] ItemDto item)
    {
        await _itemService.UpdateAsync(id, item);
        return Ok(item);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        var isDeleted = await _itemService.DeleteAsync(id);
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
        var response = await _itemService.AddBulkAsync(inputstream);
        return Ok(new { Message = response });
    }
}
