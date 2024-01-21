using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityManager.Models
{
    public class ApplicationUser : IdentityUser
    {
        //Add extra column to identity table name aspnetusers
        [Required]
        public string Name { get; set; }

        [NotMapped]
        public string RoleId { get; set; }
        [NotMapped]
        public string Role { get; set; }
    }
}
