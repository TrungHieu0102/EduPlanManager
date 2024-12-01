using System.ComponentModel.DataAnnotations;

namespace EduPlanManager.Models.Entities
{
    public class SubjectCategory
    {
        [Key]
        public Guid Id { get; set; } // Khóa chính

        [Required]
        [StringLength(10)] // Giới hạn mã danh mục
        public string Code { get; set; } // Mã danh mục

        [Required]
        [StringLength(200)] // Giới hạn tên danh mục
        public string FullName { get; set; } // Tên đầy đủ của danh mục

        // Navigation Property
        public ICollection<Subject> Subjects { get; set; } // Quan hệ 1:N với bảng Subject
    }
}
