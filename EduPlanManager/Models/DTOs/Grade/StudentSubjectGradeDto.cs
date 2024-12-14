namespace EduPlanManager.Models.DTOs.Grade
{
    public class StudentSubjectGradeDto
    {
        public string StudentFullName { get; set; } 
        public string SubjectName { get; set; } 
        public string MidtermScore { get; set; }
        public string FinalScore { get; set; } 
        public string BonusScore { get; set; } 
        public string StudentId { get; set; }
        public string SubjectId { get; set; }
    }
}
