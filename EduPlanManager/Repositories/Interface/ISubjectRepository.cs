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
    }
}
