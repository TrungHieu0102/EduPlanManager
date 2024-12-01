using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduPlanManager.Models.Entities
{
    public class Subject
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)] // Giới hạn tên môn học
        public string Code { get; set; }

        [Required]
        [StringLength(200)] // Giới hạn tên đầy đủ môn học
        public string Name { get; set; }

        [Range(1, 12)] // Giới hạn học kỳ từ 1 đến 12
        public int Semester { get; set; }

        [Range(1, 9999)] // Giới hạn năm học
        public int Year { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Range(1, 10)] // Giới hạn số tiết học mỗi ngày
        public int LessonsPerDay { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public SubjectCategory Category { get; set; }

        [Required]
        public Guid AcademicTermId { get; set; }
        [ForeignKey("AcademicTermId")] // Chỉ định rõ khóa ngoại
        public virtual AcademicTerm AcademicTerm { get; set; }

        // Navigation Properties
        public ICollection<Grade> Grades { get; set; }
        public ICollection<StudentSchedule> StudentSchedules { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
