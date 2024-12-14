using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduPlanManager.Models.Entities
{
    public class Subject
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)] 
        public string Code { get; set; }

        [Required]
        [StringLength(200)] 
        public string Name { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Range(1, 10)] 
        public int LessonsPerDay { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public SubjectCategory Category { get; set; }

        [Required]
        public Guid AcademicTermId { get; set; }
        [ForeignKey("AcademicTermId")] 
        public virtual AcademicTerm AcademicTerm { get; set; }

        // Navigation Properties
        public ICollection<Grade> Grades { get; set; }
        public ICollection<StudentSchedule> StudentSchedules { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Class> Classes { get; set; }
        public ICollection<SubjectSchedule> Schedules { get; set; } = new List<SubjectSchedule>();
        public Guid TeacherId { get; set; } 
        public User Teacher { get; set; } 


    }
}
