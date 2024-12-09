using EduPlanManager.Models.DTOs.Schedule;

namespace EduPlanManager.Services.Interface
{
    public interface IStudentScheduleService
    {
        Task<List<WeeklyScheduleDto>> GetWeeklySchedulesAsync(Guid studentId);
    }
}
