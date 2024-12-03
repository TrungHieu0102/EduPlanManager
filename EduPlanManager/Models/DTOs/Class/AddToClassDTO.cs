namespace EduPlanManager.Models.DTOs.Class
{
    public class AddToClassDTO<T> where T : class
    {
        public List<Guid> Ids { get; set; }
        public Guid ClassId { get; set; }
    }
}
