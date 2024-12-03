using System.ComponentModel.DataAnnotations;

namespace EduPlanManager.Models.DTOs.SubjectSchedule
{
    public class CreateSubjectScheduleDTO
    {
        public Guid Id { get; set; }

        [Required]
        public int DayOfWeek { get; set; }

        [Required]
        public int Session { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }
    }
}
