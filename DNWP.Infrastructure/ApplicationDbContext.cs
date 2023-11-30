using DNWP.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using static DNWP.Domain.Entities.IdentityModels;

namespace DNWP.Infrastructure;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser,
    ApplicationRole, long,
    ApplicationUserClaim,
    ApplicationUserRole,
    ApplicationUserLogin,
    ApplicationRoleClaim,
    ApplicationUserToken>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.Entity<ApplicationUserRole>().HasKey(t => new { t.UserId, t.RoleId });
        builder.Entity<ApplicationUserToken>().HasKey(t => new { t.UserId, t.Name, t.LoginProvider });
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Item> Items { get; set; }

}
