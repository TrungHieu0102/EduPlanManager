using System.ComponentModel.DataAnnotations;

namespace EduPlanManager.Models.DTOs.Class
{
    public class CreateUpdateClassDTO
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Tên lớp học không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên lớp học không được quá 100 ký tự.")]
        public string ClassName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã lớp học không được để trống.")]
        [StringLength(50, ErrorMessage = "Mã lớp học không được quá 50 ký tự.")]
        public string Code { get; set; } = string.Empty;

    }
}
