namespace EduPlanManager.Models.DTOs.Schedule
{
    public class WeeklyScheduleDto
    {
        public DateTime WeekStartDate { get; set; }
        public DateTime WeekEndDate { get; set; }
        public List<DailyScheduleDto> DailySchedules { get; set; } = new List<DailyScheduleDto>();
    }
}
