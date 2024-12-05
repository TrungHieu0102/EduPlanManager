using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EduPlanManager.Models.Entities
{
    public class Enrollment
    {
        [Key]
        public Guid Id { get; set; } // Khóa chính

        [Required]
        public Guid StudentId { get; set; } // Mã sinh viên

        [Required]
        public Guid SubjectId { get; set; } // Mã môn học
        [Required]
        public Guid SubjectScheduleId { get; set; } // Mã môn học

        [Required]
        public DateTime RegisteredAt { get; set; } = DateTime.Now; // Ngày đăng ký

        [Required]
        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending; // Trạng thái đăng ký

        [Required]
        public Guid AcademicTermId { get; set; } // FK tới AcademicTerm
        [ForeignKey("AcademicTermId")] // Chỉ định rõ khóa ngoại
        public virtual AcademicTerm AcademicTerm { get; set; } // Navigation Property

        [Required]
        public User Student { get; set; }

        [Required]
        public Subject Subject { get; set; }
        [Required]
        public SubjectSchedule SubjectSchedule { get; set; }
    }
    public enum EnrollmentStatus
    {
        Pending,     // Đang chờ phê duyệt
        Approved,    // Đã phê duyệt
        Rejected,    // Bị từ chối
        Cancelled    // Đã hủy
    }
}
