using System.ComponentModel.DataAnnotations;

namespace EduPlanManager.Models.Entities
{
    public class SubjectSchedule
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DayOfWeekEnum DayOfWeek { get; set; }
        public SessionEnum Session { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    }
    public enum DayOfWeekEnum
    {
        [Display(Name = "Chủ nhật")]
        Sunday = 0,

        [Display(Name = "Thứ Hai")]
        Monday = 1,

        [Display(Name = "Thứ Ba")]
        Tuesday = 2,

        [Display(Name = "Thứ Tư")]
        Wednesday = 3,

        [Display(Name = "Thứ Năm")]
        Thursday = 4,

        [Display(Name = "Thứ Sáu")]
        Friday = 5,

        [Display(Name = "Thứ Bảy")]
        Saturday = 6
    }
    public enum SessionEnum
    {
        [Display(Name = "Sáng")]
        Morning = 0,

        [Display(Name = "Chiều")]
        Afternoon = 1,

        [Display(Name = "Tối")]
        Evening = 2
    }
}
