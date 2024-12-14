using EduPlanManager.Models.Entities;

namespace EduPlanManager.Models.DTOs.Grade
{
    public class CreateGradeDto
    {
        public Guid StudentId { get; set; }  // The unique identifier for the student
        public Guid SubjectId { get; set; }  // The unique identifier for the subject
        public GradeType GradeType { get; set; }  // The type of grade (Midterm, Final, Bonus)
        public decimal Score { get; set; }  // The score for the selected grade type
    }
}
