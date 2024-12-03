namespace EduPlanManager.Models.DTOs.User;

public class UserDetailsDto
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string ?Email { get; set; }
    public string ?PhoneNumber { get; set; }
    public string? Address { get; set; }

    public List<string>? Roles { get; set; } = new List<string>();
}
