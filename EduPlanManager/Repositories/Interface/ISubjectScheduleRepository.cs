using EduPlanManager.Models.Entities;

namespace EduPlanManager.Repositories.Interface
{
    public interface ISubjectScheduleRepository : IRepositoryBase<SubjectSchedule, Guid>
    {
        Task UpdateAsync(SubjectSchedule schedule);
        Task DeleteAsync(SubjectSchedule schedule);
        Task<bool> IsDuplicateScheduleAsync(int? dayOfWeek, int? session);
    }
}
