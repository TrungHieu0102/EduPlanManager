using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EduPlanManager.Models.Entities
{
    public class Role : IdentityRole<Guid>
    {
        [Required]
        [MaxLength(200)]
        public required string DisplayName { get; set; }
    }
}
