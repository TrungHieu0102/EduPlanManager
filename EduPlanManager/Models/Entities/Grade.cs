using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduPlanManager.Models.Entities
{
    public class Grade
    {
        [Key]
        public Guid Id { get; set; } 

        [Required]
        public Guid StudentId { get; set; } 

        [Required]
        public Guid SubjectId { get; set; } // ID môn học

        [Required]
        public GradeType Type { get; set; } // Loại điểm (giữa kỳ, cuối kỳ, v.v.)

        [Range(0, 10)] // Giới hạn điểm số từ 0 đến 10
        public float Score { get; set; } // Điểm số

        [Required]
        public Guid AcademicTermId { get; set; } // FK tới AcademicTerm
        [ForeignKey("AcademicTermId")] // Chỉ định rõ khóa ngoại
        public virtual AcademicTerm AcademicTerm { get; set; } // Navigation Property

        // Navigation Properties
        [Required]
        public User Student { get; set; }

        [Required]
        public Subject Subject { get; set; }
    }

    public enum GradeType
    {
        Midterm = 1,    
        Final = 2,      
        Bonus = 3       
    }
}
