using DNWP.Domain.Entities;
using DNWP.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static DNWP.Domain.Entities.IdentityModels;

namespace DNWP.API.Dependencies;

public static class DbContextDependency
{
    public static void AddDbContextDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        var dbConnectionString = configuration.GetConnectionString("ApplicationDb");
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            options.UseSqlServer(dbConnectionString);
        });

        services
         .AddIdentity<ApplicationUser, ApplicationRole>(options =>
         {
             options.Password.RequiredLength = 6;
             options.Password.RequireNonAlphanumeric = false;
             options.Password.RequireDigit = true;
             options.Password.RequireLowercase = true;
             options.Password.RequireUppercase = false;
         })
         .AddEntityFrameworkStores<ApplicationDbContext>()
         .AddDefaultTokenProviders();

        using (var serviceProvider = services.BuildServiceProvider())
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();
        }

    }
}