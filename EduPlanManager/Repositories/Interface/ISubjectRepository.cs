using EduPlanManager.Data;
using EduPlanManager.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduPlanManager.Repositories.Interface
{
    public interface ISubjectRepository : IRepositoryBase<Subject, Guid>
    {
        Task<int> GetTotalSubjectsAsync(string searchTerm, int? semester, int? year);
        Task<Subject> GetSubjectWithDetailsAsync(Guid id);
        IQueryable<Subject> GetQueryable();
        Task<List<Subject>> GetSubjectsByIdsAsync(List<Guid> ids);
        Task DeleteSubjectsAsync(List<Subject> subjects);
        Task<List<Subject>> GetSubjectsClassAsync(bool isHaveClass, Guid classId);
        Task<List<Subject>> GetSubjectsScheduleAsync(bool isHaveSchedule, Guid scheduleId);
        Task<bool> IsSubjectExistsInformation(string code, Guid academicTermId, Guid teacherId);
        Task<List<Subject>> GetAllSubjectByUserId(Guid userId);
    }
}
