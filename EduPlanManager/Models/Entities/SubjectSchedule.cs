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
        Sunday = 0,    // Chủ nhật
        Monday = 1,    // Thứ Hai
        Tuesday = 2,   // Thứ Ba
        Wednesday = 3, // Thứ Tư
        Thursday = 4,  // Thứ Năm
        Friday = 5,    // Thứ Sáu
        Saturday = 6   // Thứ Bảy
    }
    public enum SessionEnum
    {
        Morning = 0,  // Sáng
        Afternoon = 1, // Chiều
        Evening = 2   // Tối
    }
}
