using System.ComponentModel.DataAnnotations;

namespace EduPlanManager.Models.DTOs.Enrollment
{
    public class EnrollmentRequest
    {
        [Required]
        public Guid StudentId { get; set; } // Mã sinh viên

        [Required]
        public Guid SubjectId { get; set; } // Mã môn học

        [Required]
        public Guid SubjectScheduleId { get; set; } // Mã lịch học của môn học

        [Required]
        public Guid AcademicTermId { get; set; } // Mã học kỳ
         
    }
}
