using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DNWP.Domain.Entities;

[Table("AspNetUsers")]
public class ApplicationUser : IdentityUser<long>
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; }

    [MaxLength(100)]
    public string LastName { get; set; }

    [Required]
    public DateTime CreatedDateUtc { get; set; }

    public DateTime? UpdatedDateUtc { get; set; }

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    [NotMapped]
    public string FullNameWithEmail
    {
        get
        {
            StringBuilder formmattedName = new StringBuilder();
            if (!string.IsNullOrEmpty(FirstName))
            {
                formmattedName.Append(FirstName);
            }
            if (!string.IsNullOrEmpty(LastName))
            {
                if (formmattedName.Length != 0)
                {
                    formmattedName.Append(" ");
                }
                formmattedName.Append(LastName);
            }
            if (!string.IsNullOrEmpty(Email))
            {
                formmattedName.Append(" (" + Email + ")");
            }
            return formmattedName.ToString();

        }
    }
}
