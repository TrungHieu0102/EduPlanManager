using EduPlanManager.Models.Entities;

namespace EduPlanManager.Repositories.Interface
{
    public interface IStudentScheduleRepository : IRepositoryBase<StudentSchedule, Guid>
    {
        Task AddStudentScheduleAsync(StudentSchedule studentSchedule);
    }
}
