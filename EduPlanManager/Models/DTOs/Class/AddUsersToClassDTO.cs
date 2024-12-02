namespace EduPlanManager.Models.DTOs.Class
{
    public class AddUsersToClassDTO
    {
        public List<Guid> UserId { get; set; }
        public Guid ClassId { get; set; }
    }
}
