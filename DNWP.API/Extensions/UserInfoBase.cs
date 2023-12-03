using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DNWP.API.Extensions;

public class UserInfoBase : ControllerBase
{
    public long? UserId
    {
        get => User.Identity?.IsAuthenticated ?? false ? long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) : null;
    }
    public string UserEmail
    {
        get => User.Identity?.IsAuthenticated ?? false ? User.FindFirstValue(ClaimTypes.Email) : null;
    }
    public string MobileNumber
    {
        get => User.Identity?.IsAuthenticated ?? false ? User.FindFirstValue(ClaimTypes.MobilePhone) : null;
    }
    public string UserName
    {
        get => User.Identity?.IsAuthenticated ?? false ? User.Identity.Name : null;
    }

    public string UserFullName
    {
        get => User.Identity?.IsAuthenticated ?? false ? User.FindFirstValue(ClaimTypes.GivenName) : null;
    }
    public List<string> Roles
    {
        get => User.Identity?.IsAuthenticated ?? false ? User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList() : null;
    }
}
