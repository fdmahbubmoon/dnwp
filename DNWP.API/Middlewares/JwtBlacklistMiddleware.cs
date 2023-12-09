using DNWP.Application.Interfaces;
using NuGet.Protocol;

namespace DNWP.API.Middlewares;

public class JwtBlacklistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITokenBlacklistManager _tokenBlacklistManager;

    public JwtBlacklistMiddleware(RequestDelegate next, ITokenBlacklistManager tokenBlacklistManager)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _tokenBlacklistManager = tokenBlacklistManager;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token) && _tokenBlacklistManager.GetBucket().Exists(b=>b.Token == token))
        {
            context.Response.StatusCode = 401;
            return;
        }

        await _next(context);
    }
}
