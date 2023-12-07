using DNWP.Application.Interfaces;
using DNWP.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DNWP.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    public AuthController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> GenerateTokenAsync([FromBody] LoginDto model)
    {
        try
        {
            return Created(string.Empty, await _tokenService.GenerateTokenAsync(model.UserName, model.Password));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message ?? ex.InnerException?.Message);
        }
    }

    [Authorize]
    [HttpGet("sign-out")]
    public async Task<IActionResult> SignoutAsync()
    {
        try
        {
            await _tokenService.SignoutAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message ?? ex.InnerException?.Message);
        }
    }
}
