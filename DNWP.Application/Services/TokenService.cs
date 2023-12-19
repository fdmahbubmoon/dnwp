using DNWP.Application.Interfaces;
using DNWP.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DNWP.Application.Exceptions;
using DNWP.Application.Helpers;
using DNWP.Application.Settings;

namespace DNWP.Application.Services;

public class TokenVm
{
    public string Access_token { get; set; }
    public string UserName { get; set; }
    public string FullName { get; set; }
    public DateTimeOffset Expires { get; set; }
    public string Token_type { get; set; }
    public string[] Roles { get; set; }
}

public class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILoggedInUserService _loggedInUserService;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly Jwt _jwtSettings;
    private readonly ITokenBlacklistManager _tokenBlocklistManager;
    public TokenService(
        UserManager<ApplicationUser> userManager,
        IOptionsSnapshot<Jwt> jwtSettings,
        ILoggedInUserService loggedInUserService,
        SignInManager<ApplicationUser> signInManager,
        ITokenBlacklistManager tokenBlocklistManager)
    {
        _userManager = userManager;
        _loggedInUserService = loggedInUserService;
        _signInManager = signInManager;
        _jwtSettings = jwtSettings.Value;
        _tokenBlocklistManager = tokenBlocklistManager;
    }

    public async Task<TokenVm> GenerateTokenAsync(string userName, string password)
    {
        ApplicationUser user = await _userManager.FindByNameAsync(userName);

        if (!await CheckPasswordAsync(password, user))
            throw new BadRequestException("Invalid Credentials");

        List<Claim> authClaims = await GetClaimsAsync(user);

        var token = await GenerateToken(user, authClaims);
        return token;
    }

    private async Task<bool> CheckPasswordAsync(string password, ApplicationUser user)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    private async Task<List<Claim>> GetClaimsAsync(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
                {
                    new (ClaimTypes.GivenName, user.FullName),
                    new (ClaimTypes.Name, user.UserName),
                    new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                };

        if (!string.IsNullOrEmpty(user.PhoneNumber))
            authClaims.Add(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));

        userRoles.ToList().ForEach(r => authClaims.Add(new Claim(ClaimTypes.Role, r)));

        return authClaims;
    }

    private async Task<TokenVm> GenerateToken(ApplicationUser user, List<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, _jwtSettings.Alg);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ValidForInMinitues),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        var tokenVm = new TokenVm
        {
            UserName = user.UserName,
            FullName = user.FullName,
            Access_token = tokenString,
            Expires = token.ValidTo,
            Token_type = "Bearer",
            Roles = claims.Where(claim => claim.Type == ClaimTypes.Role).Select(s=>s.Value).ToArray()
        };
        return tokenVm;
    }

    public async Task SignoutAsync()
    {
        long? userId = _loggedInUserService.UserId;
        if (userId is not null)
        {
            _tokenBlocklistManager.RemoveExpired();
            _tokenBlocklistManager.Add(_loggedInUserService.AccessToken, _loggedInUserService.JwtExpiresAt);
            await _signInManager.SignOutAsync();
        }
    }
}