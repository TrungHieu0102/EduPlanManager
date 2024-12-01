using System.ComponentModel.DataAnnotations;

namespace EduPlanManager.Models.DTOs.Subject
{
    public class SubjectCreateDTO
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Mã môn học là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Mã môn học không được vượt quá 50 ký tự.")]
        public string? Code { get; set; }

        [Required(ErrorMessage = "Tên môn học là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên môn học không được vượt quá 100 ký tự.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc.")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(SubjectCreateDTO), nameof(ValidateEndDate))]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Số tiết mỗi buổi là bắt buộc.")]
        [Range(1, 10, ErrorMessage = "Số tiết mỗi buổi phải nằm trong khoảng từ 1 đến 10.")]
        public int LessonsPerDay { get; set; }

        [Required(ErrorMessage = "Danh mục môn học là bắt buộc.")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Kỳ học là bắt buộc.")]
        public Guid AcademicTermId { get; set; }

        // Custom validation for EndDate
        public static ValidationResult? ValidateEndDate(DateTime endDate, ValidationContext context)
        {
            var instance = context.ObjectInstance as SubjectCreateDTO;

            if (instance != null && instance.StartDate > endDate)
            {
                return new ValidationResult("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");
            }

            return ValidationResult.Success;
        }
    }
}
