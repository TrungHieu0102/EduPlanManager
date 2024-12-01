using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduPlanManager.Models.Entities
{
    public class EnrollmentRequest
    {
        [Key]
        public Guid Id { get; set; } // Khóa chính

        [Required]
        public Guid StudentId { get; set; } // Mã sinh viên

        [Required]
        public Guid SubjectId { get; set; } // Mã môn học

        [Required]
        public RequestStatus Status { get; set; } = RequestStatus.Pending; // Trạng thái yêu cầu

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Thời gian tạo

        public DateTime? CancelledAt { get; set; } // Thời gian hủy (nếu có)

        // Navigation Properties
        [Required]
        public User Student { get; set; }

        [Required]
        public Subject Subject { get; set; }
    }

    public enum RequestStatus
    {
        Pending,    // Đang chờ xử lý
        Approved,   // Đã phê duyệt
        Rejected    // Bị từ chối
    }
}
