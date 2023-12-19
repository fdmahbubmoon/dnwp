using System.Reflection;
using DNWP.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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

        #region Seed Users & Roles
        //Seeding a  'Administrator' role to AspNetRoles table
        builder.Entity<ApplicationRole>().HasData(
            new ApplicationRole { Id = 1, Name = "Admin", NormalizedName = "ADMIN", CreatedDateUtc = DateTime.UtcNow },
            new ApplicationRole { Id = 2, Name = "General", NormalizedName = "GENERAL", CreatedDateUtc = DateTime.UtcNow }
            );


        //a hasher to hash the password before seeding the user to the db
        var hasher = new PasswordHasher<IdentityUser>();


        //Seeding the User to AspNetUsers table
        builder.Entity<ApplicationUser>().HasData(
            new ApplicationUser
            {
                Id = 1,
                UserName = "admin1",
                NormalizedUserName = "ADMIN1",
                PasswordHash = hasher.HashPassword(null, "Admin123"),
                CreatedDateUtc = DateTime.UtcNow,
                FirstName = "Admin",
                LastName = "1"
            },
            new ApplicationUser
            {
                Id = 2,
                UserName = "general1",
                NormalizedUserName = "GENERAL1",
                PasswordHash = hasher.HashPassword(null, "General123"),
                CreatedDateUtc = DateTime.UtcNow,
                FirstName = "General",
                LastName = "1"
            }
        );


        //Seeding the relation between our user and role to AspNetUserRoles table
        builder.Entity<ApplicationUserRole>().HasData(
            new ApplicationUserRole
            {
                RoleId = 1,
                UserId = 1
            },
            new ApplicationUserRole
            {
                RoleId = 2,
                UserId = 2
            }
        );

        #endregion

    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Item> Items { get; set; }

}
