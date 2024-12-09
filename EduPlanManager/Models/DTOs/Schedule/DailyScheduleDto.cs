namespace EduPlanManager.Models.DTOs.Schedule
{
    public class DailyScheduleDto
    {
        public DayOfWeek DayOfWeek { get; set; }
        public string SubjectName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
