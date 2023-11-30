using DNWP.Common.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace DNWP.API.Extensions;

public static class JwtExtension
{
    public static void AddJWT(this IServiceCollection services, IConfiguration configuration)
    {
        var token = configuration.GetSection("AppSettings:Jwt").Get<Jwt>();

        var validationParameters = new TokenValidationParameters
        {
            ValidAudience = token.Audience,
            ValidIssuer = token.Issuer,
            ValidateAudience = token.IsValidateAudience,
            ValidateIssuer = token.IsValidateIssuer,
            ValidateIssuerSigningKey = token.IsValidateIssuerSigningKey,
            ValidateLifetime = token.IsValidateLifetime,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = ClaimTypes.Role,
            IssuerSigningKey =
           new SymmetricSecurityKey(Encoding.UTF8.GetBytes(token.SecretKey)),
        };

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;
                options.TokenValidationParameters = validationParameters;
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        if (context.Request.Path.Value.Contains("hub") &&
                            !string.IsNullOrWhiteSpace(accessToken))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };

            });
    }
}