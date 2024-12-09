using EduPlanManager.Models.DTOs.Schedule;

namespace EduPlanManager.Models.ViewModels
{
    public class WeeklyScheduleViewModel
    {
        public WeeklyScheduleDto WeeklySchedule { get; set; } // Lịch học cho tuần hiện tại
        public int CurrentPage { get; set; } // Trang hiện tại (tuần hiện tại)
        public int TotalPages { get; set; } // Tổng số tuần
    }
}
