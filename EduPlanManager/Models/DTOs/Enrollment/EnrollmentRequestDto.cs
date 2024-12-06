using EduPlanManager.Models.Entities;

namespace EduPlanManager.Models.DTOs.Enrollment
{
    public class EnrollmentRequestDto
    {
        public Guid EnrollmentId { get; set; }
        public string StudentName { get; set; }
        public string SubjectName { get; set; }
        public DayOfWeekEnum DayOfWeek { get; set; }
        public SessionEnum Session { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; }
    }
}
