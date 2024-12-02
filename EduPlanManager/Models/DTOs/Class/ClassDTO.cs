namespace EduPlanManager.Models.DTOs.Class
{
    public class ClassDTO
    {
        public string Id { get; set; } = string.Empty;  
        public string ClassName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public int TeacherCount { get; set; }
    }
}
