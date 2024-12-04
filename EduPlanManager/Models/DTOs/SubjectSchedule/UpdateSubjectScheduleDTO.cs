using EduPlanManager.Extentions;
using EduPlanManager.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace EduPlanManager.Models.DTOs.SubjectSchedule
{
    public class UpdateSubjectScheduleDTO
    {
        public Guid Id { get; set; }

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
        [CompareStartTime(ErrorMessage = "Giờ kết thúc phải sau giờ bắt đầu.")]
        public TimeSpan EndTime { get; set; }
        public string DayOfWeekName => ((DayOfWeekEnum)DayOfWeek).GetDisplayName();

        // Thuộc tính để lấy tên hiển thị của Session
        public string SessionName => ((SessionEnum)Session).GetDisplayName();
    }
    public class CompareStartTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (UpdateSubjectScheduleDTO)validationContext.ObjectInstance;

            if (model.EndTime <= model.StartTime)
            {
                return new ValidationResult("Giờ kết thúc phải sau giờ bắt đầu.");
            }
            return ValidationResult.Success;
        }
    }
}
