using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace DNWP.Application.Helpers;

public class LoggedInUserService : ILoggedInUserService
{

    public LoggedInUserService(IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated ?? false)
        {
            var user = httpContextAccessor.HttpContext.User;
            UserId = long.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);
            FullName = user.FindFirst(ClaimTypes.GivenName).Value;
            UserName = user.Identity.Name;
            IsAuthenticated = user.Identity.IsAuthenticated;
            Roles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            JwtExpiresAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(user.FindFirst("exp").Value));
        }

        var request = httpContextAccessor?.HttpContext?.Request;
        AccessToken = request?.Headers["Authorization"];
        RequestOrigin = request?.Headers["Origin"].ToString()?.Trim();
        AccessToken = AccessToken != "null" ? AccessToken?.Split(" ")[1] : default;
    }

    public long? UserId { get; }
    public string UserEmail { get; }
    public string FullName { get; }
    public string MobileNumber { get; }
    public string UserName { get; }
    public List<string> Roles { get; }
    public bool IsHeadOfficeUser { get; }
    public bool IsAuthenticated { get; }
    public string AccessToken { get; }
    public DateTimeOffset JwtExpiresAt { get; }
    public string RequestOrigin { get; }
}