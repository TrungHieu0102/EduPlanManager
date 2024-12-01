using System;
using System.ComponentModel.DataAnnotations;

namespace EduPlanManager.Models.DTOs.AcademicTerm
{
    public class CreateUpdateAcademicTermDTO
    {
        [Required(ErrorMessage = "Id không được để trống.")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Năm học không được để trống.")]
        [Range(2000, 2100, ErrorMessage = "Năm học phải nằm trong khoảng từ 2000 đến 2100.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Học kỳ không được để trống.")]
        [Range(1, 3, ErrorMessage = "Học kỳ chỉ có thể là 1, 2 hoặc 3.")]
        public int Semester { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống.")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(CreateUpdateAcademicTermDTO), nameof(ValidateStartDate))]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc không được để trống.")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(CreateUpdateAcademicTermDTO), nameof(ValidateEndDate))]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Validate ngày bắt đầu.
        /// </summary>
        public static ValidationResult ValidateStartDate(DateTime startDate, ValidationContext context)
        {
            var instance = (CreateUpdateAcademicTermDTO)context.ObjectInstance;
            if (startDate.Year != instance.Year)
            {
                return new ValidationResult("Ngày bắt đầu phải thuộc năm học đã chọn.");
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Validate ngày kết thúc.
        /// </summary>
        public static ValidationResult ValidateEndDate(DateTime endDate, ValidationContext context)
        {
            var instance = (CreateUpdateAcademicTermDTO)context.ObjectInstance;
            if (endDate < instance.StartDate)
            {
                return new ValidationResult("Ngày kết thúc phải sau ngày bắt đầu.");
            }

            if (endDate.Year > instance.Year + 1)
            {
                return new ValidationResult("Ngày kết thúc không được vượt quá năm tiếp theo của năm học.");
            }

            return ValidationResult.Success;
        }
    }
}
