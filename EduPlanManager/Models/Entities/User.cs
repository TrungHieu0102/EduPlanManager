using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EduPlanManager.Models.Entities
{
    public class User : IdentityUser<Guid>
    {
        [Required]
        [MaxLength(100)]
        public required string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public required string LastName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string GetFullName()
        {
            return this.FirstName + " " + this.LastName;
        }
    }
}
