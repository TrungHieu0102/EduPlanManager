using EduPlanManager.Extentions;
using EduPlanManager.Models.Entities;

namespace EduPlanManager.Models.DTOs.SubjectSchedule
{
    public class SubjectScheduleDTO
    {
        public Guid Id { get; set; }
        public int DayOfWeek { get; set; }
        public int Session { get; set; }  
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        // Thuộc tính để lấy tên của DayOfWeek
        public string DayOfWeekName =>  ((DayOfWeekEnum)DayOfWeek).GetDisplayName() ;

        // Thuộc tính để lấy tên hiển thị của Session
        public string SessionName =>  ((SessionEnum)Session).GetDisplayName() ;
    }
}
