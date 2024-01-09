using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IdentityManager.Models
{
    public class ApplicationUser : IdentityUser
    {
        //Add extra column to identity table name aspnetusers
        [Required]
        public string Name { get; set; }
    }
}
