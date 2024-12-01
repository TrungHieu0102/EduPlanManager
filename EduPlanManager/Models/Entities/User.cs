using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EduPlanManager.Models.Entities
{
    public class User : IdentityUser<Guid>
    {
        [Required]
        [MaxLength(100)]
        public required string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public required string LastName { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? ProfilePicture { get; set; }

        [MaxLength(100)]
        public string? Title { get; set; }

        [MaxLength(50)]
        public string? AccountStatus { get; set; }

        public string? Gender { get; set; }

        public string? AdditionalInfo { get; set; }

        // Navigation Properties
        public ICollection<Grade> Grades { get; set; } // Liên kết tới điểm số
        public ICollection<StudentSchedule> Schedules { get; set; } // Liên kết tới thời khóa biểu
        public ICollection<Enrollment> Enrollments { get; set; } // Quan hệ với bảng đăng ký môn học

        public string GetFullName()
        {
            return this.FirstName + " " + this.LastName;
        }
    }
}
