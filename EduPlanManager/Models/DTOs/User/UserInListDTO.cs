namespace EduPlanManager.Models.DTOs.User
{
    public class UserInListDTO
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ClassName { get; set; }
    }
}
