using DNWP.Application.Interfaces;
using DNWP.Domain.Entities;
using DNWP.Domain.Models;
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
        await _itemService.AddAsyc(item);
        return Ok(item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(long id, [FromBody] ItemDto item)
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
}
