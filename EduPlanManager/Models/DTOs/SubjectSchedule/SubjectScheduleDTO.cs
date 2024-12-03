namespace EduPlanManager.Models.DTOs.SubjectSchedule
{
    public class SubjectScheduleDTO
    {
        public Guid Id { get; set; }
        public int? DayOfWeek { get; set; }
        public int? Session { get; set; }  
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}
