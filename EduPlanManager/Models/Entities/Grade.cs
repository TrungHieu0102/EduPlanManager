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
        public Guid SubjectId { get; set; }

        [Required]
        public GradeType Type { get; set; } 

        [Range(0, 10)]
        public float Score { get; set; } 

        [Required]
        public Guid AcademicTermId { get; set; } 
        [ForeignKey("AcademicTermId")] 
        public virtual AcademicTerm AcademicTerm { get; set; } 

        [Required]
        public User Student { get; set; }

        [Required]
        public Subject Subject { get; set; }
        [Required]
        public Guid SumaryGradeId { get; set; }

        [ForeignKey("SumaryGradeId")]
        public virtual SumaryGrade SumaryGrade { get; set; }
    }

    public enum GradeType
    {
        Midterm = 1,    
        Final = 2,      
        Bonus = 3       
    }
}
