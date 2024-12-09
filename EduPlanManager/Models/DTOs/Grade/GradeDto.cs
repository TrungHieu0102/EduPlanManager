namespace EduPlanManager.Models.DTOs.Grade
{
    public class GradeDto
    {
        public string? StudentName { get; set; }
        public string? SubjectName { get; set; }
        public int AcademicYear { get; set; }    
        public int AcademicSemester { get; set; }
        public double GradeValue { get; set; }
        public string ?GradeType { get; set; }
    }
}
