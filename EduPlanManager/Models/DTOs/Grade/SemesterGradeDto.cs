namespace EduPlanManager.Models.DTOs.Grade
{
    public class SemesterGradeDto
    {
        public int AcademicYear { get; set; } // Năm học
        public int AcademicSemester { get; set; } // Học kỳ
        public List<SubjectGradeDto> SubjectGrades { get; set; } = new List<SubjectGradeDto>();
    }
}
