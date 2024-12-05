using EduPlanManager.Models.Entities;

namespace EduPlanManager.Models.DTOs.Enrollment
{
    public class EnrollmentListRespone
    {
        public string SubjectCode { get; set; }
        public Guid SubjectId { get; set; }
        public Guid ScheduleId { get; set; }
        public string SubjectName { get; set; }
        public DayOfWeekEnum DayOfWeek { get; set; }
        public SessionEnum Session { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
