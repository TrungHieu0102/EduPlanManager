using System.ComponentModel.DataAnnotations;

namespace EduPlanManager.Models.DTOs.User
{
    public class CreateUserDto
    {

        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [MaxLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự.")]
        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
