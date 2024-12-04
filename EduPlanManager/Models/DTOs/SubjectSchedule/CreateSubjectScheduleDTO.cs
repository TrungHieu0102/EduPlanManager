using System.ComponentModel.DataAnnotations;

namespace EduPlanManager.Models.DTOs.SubjectSchedule
{
    public class CreateSubjectScheduleDTO
    {
        [Required(ErrorMessage = "Vui lòng chọn ngày trong tuần.")]
        [Range(0, 6, ErrorMessage = "Ngày trong tuần phải từ Chủ nhật (0) đến Thứ Bảy (6).")]
        public int DayOfWeek { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn buổi học.")]
        [Range(0, 2, ErrorMessage = "Buổi học phải từ Sáng (0) đến Tối (2).")]
        public int Session { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giờ bắt đầu.")]
        [DataType(DataType.Time, ErrorMessage = "Giờ bắt đầu không hợp lệ.")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giờ kết thúc.")]
        [DataType(DataType.Time, ErrorMessage = "Giờ kết thúc không hợp lệ.")]
        [CompareCreateStartTime(ErrorMessage = "Giờ kết thúc phải sau giờ bắt đầu.")]
        public TimeSpan EndTime { get; set; }
    }
    public class CompareCreateStartTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (CreateSubjectScheduleDTO)validationContext.ObjectInstance;

            if (model.EndTime <= model.StartTime)
            {
                return new ValidationResult("Giờ kết thúc phải sau giờ bắt đầu.");
            }
            return ValidationResult.Success;
        }
    }
}
