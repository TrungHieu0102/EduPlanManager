using System.ComponentModel.DataAnnotations;

namespace EduPlanManager.Models.DTOs.Enrollment
{
    public class EnrollmentRequest
    {
        [Required]
        public Guid StudentId { get; set; } 

        [Required]
        public Guid SubjectId { get; set; } 

        [Required]
        public Guid SubjectScheduleId { get; set; }

        [Required]
        public Guid AcademicTermId { get; set; }
         
    }
}
