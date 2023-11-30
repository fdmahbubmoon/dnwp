using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DNWP.Domain.Entities;
public class IdentityModels
{
    [Table("AspNetUserRoles")]
    public class ApplicationUserRole : IdentityUserRole<long>
    {
    }

    [Table("AspNetUserClaims")]
    public class ApplicationUserClaim : IdentityUserClaim<long>
    {
    }

    public class ApplicationUserLogin : IdentityUserLogin<long>
    {
        [ForeignKey("ApplicationUser"), Key]
        public override long UserId { get => base.UserId; set => base.UserId = value; }
        public ApplicationUser ApplicationUser { get; set; }
    }

    [NotMapped]
    public class ApplicationRoleClaim : IdentityRoleClaim<long>
    {
    }

    [Table("AspNetUserTokens")]
    public class ApplicationUserToken : IdentityUserToken<long>
    {
    }

    [Table("AspNetRoles")]
    public class ApplicationRole : IdentityRole<long>
    {
        public ApplicationRole() { }
        public ApplicationRole(string name) { Name = name; }

        [Required]
        public int StatusId { get; set; }
        public string Description { get; set; }

        [Required]
        public long CreatedBy { get; set; }

        [Required]
        public DateTime CreatedDateUtc { get; set; }

        public long UpdatedBy { get; set; }
        public DateTime? UpdatedDateUtc { get; set; }
    }
}
