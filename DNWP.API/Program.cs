using DNWP.API.Dependencies;
using DNWP.API.Extensions;
using DNWP.API.Middlewares;
using DNWP.Application.Interfaces;
using DNWP.Application.Services;
using DNWP.Domain.Entities;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using System.Text.Json;
using DNWP.Application.Helpers;
using DNWP.Application.RepositoryInterfaces;
using DNWP.Application.Settings;
using DNWP.Infrastructure.Repositories;
using static DNWP.Domain.Entities.IdentityModels;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services
    .AddOptions<Jwt>()
    .BindConfiguration("Jwt")
    .ValidateOnStart();

builder.Services.AddCors(options =>
{
    options.AddPolicy("all",
        builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddMemoryCache();
builder.Services.AddDbContextDependencies(builder.Configuration);
builder.Services.AddJWT(builder.Configuration);
builder.Services.AddTransient<ILoggedInUserService, LoggedInUserService>();
builder.Services.AddSingleton<ITokenBlacklistManager, TokenBlacklistManager>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped(typeof(IBaseServiceFactory<>), typeof(BaseServiceFactory<>));
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IItemService, ItemService>();
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<JwtBlacklistMiddleware>();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.ContentType = Text.Plain;

        var exception =
            context.Features.Get<IExceptionHandlerPathFeature>();

        FluentValidation.ValidationException fluentException = exception.Error as FluentValidation.ValidationException;

        if (fluentException is not null)
        {
            var response = fluentException.Errors.Select(a => new { Message = a.ErrorMessage }).ToList();

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response)
                );
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync(
                exception.Error.Message ??
                exception.Error.InnerException.Message);
        }
    });
});



app.UseHttpsRedirection();

app.UseCors("all");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
