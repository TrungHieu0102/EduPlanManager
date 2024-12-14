namespace EduPlanManager.Models.DTOs.Enrollment
{
    public class EnrollmentStudentListResponse
    {
        public Guid EnrollmentId { get; set; }
        public Guid StudentId { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string DayOfWeek { get; set; }
        public string Session { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public DateTime RegisteredAt { get; set; }
        public string Status { get; set; }
        public string TeacherName { get; set; } 
    }
}
