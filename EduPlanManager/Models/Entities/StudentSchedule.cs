using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduPlanManager.Models.Entities
{
    public class StudentSchedule
    {
        [Key]
        public Guid Id { get; set; } // ID thời khóa biểu

        [Required]
        public Guid StudentId { get; set; } // ID sinh viên

        [Required]
        public Guid SubjectId { get; set; } // ID môn học

        [Required]
        [StringLength(20)] // Giới hạn chiều dài tên ngày
        public string DayOfWeek { get; set; } // Ngày học trong tuần (VD: Monday, Tuesday)

        [Range(1, 12)] // Giới hạn học kỳ từ 1 đến 12
        public int Semester { get; set; } // Học kỳ

        [Range(1, 9999)] // Giới hạn năm học
        public int Year { get; set; } // Năm học

        [Required]
        public TimeSpan StartTime { get; set; } // Thời gian bắt đầu học (VD: 7:00)

        [Required]
        public TimeSpan EndTime { get; set; } // Thời gian kết thúc học (VD: 11:00)

        [Required]
        public Guid AcademicTermId { get; set; } // FK tới AcademicTerm
        [ForeignKey("AcademicTermId")] // Chỉ định rõ khóa ngoại
        public virtual AcademicTerm AcademicTerm { get; set; } // Navigation Property

        // Navigation Properties
        [Required]
        public User Student { get; set; } // Liên kết tới sinh viên

        [Required]
        public Subject Subject { get; set; } // Liên kết tới môn học
    }
}
