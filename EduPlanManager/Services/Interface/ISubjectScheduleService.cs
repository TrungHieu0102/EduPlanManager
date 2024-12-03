using EduPlanManager.Models.DTOs.Respone;
using EduPlanManager.Models.DTOs.SubjectSchedule;

namespace EduPlanManager.Services.Interface
{
    public interface ISubjectScheduleService
    {
        Task<Result<SubjectScheduleDTO>> CreateScheduleAsync(CreateSubjectScheduleDTO dto);
        Task<Result<SubjectScheduleDTO>> UpdateScheduleAsync(UpdateSubjectScheduleDTO dto);
        Task<Result<bool>> DeleteScheduleAsync(Guid id);
        Task<Result<IEnumerable<SubjectScheduleDTO>>> GetAllSchedulesAsync();
        Task<Result<SubjectScheduleDTO>> GetScheduleByIdAsync(Guid id);
    }
}
