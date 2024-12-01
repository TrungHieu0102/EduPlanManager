using EduPlanManager.Data;
using EduPlanManager.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduPlanManager.Repositories.Interface
{
    public interface ISubjectRepository : IRepositoryBase<Subject, Guid>
    {
        Task<IEnumerable<Subject>> GetSubjectsAsync(string searchTerm, int? semester, int? year, int pageNumber, int pageSize);
        Task<int> GetTotalSubjectsAsync(string searchTerm, int? semester, int? year);
        Task<Subject> GetSubjectWithDetailsAsync(Guid id);
        IQueryable<Subject> GetQueryable();
    }
}
