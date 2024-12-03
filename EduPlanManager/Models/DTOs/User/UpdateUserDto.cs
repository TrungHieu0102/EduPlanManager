namespace EduPlanManager.Models.DTOs.User
{
    public class UpdateUserDto
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public IFormFile? ProfilePicture { get; set; } 
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? PictureUrl { get; set; }
        public List<string>? Roles { get; set; }

    }
}
