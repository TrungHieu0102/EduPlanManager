namespace EduPlanManager.Models.DTOs.Subject
{
    public class SubjectDTO
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? AcademicTermYear { get; set; }
        public string? AcademicTermSemester { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int LessonsPerDay { get; set; }
    }
}
