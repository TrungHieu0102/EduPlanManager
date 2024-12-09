namespace EduPlanManager.Models.DTOs.Grade
{
    public class GroupedGradeDto
    {
        public string SubjectName { get; set; }
        public List<GradeDto> Grades { get; set; }
    }
}
