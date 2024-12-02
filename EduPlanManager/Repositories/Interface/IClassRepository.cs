using EduPlanManager.Models.Entities;

namespace EduPlanManager.Repositories.Interface
{
    public interface IClassRepository : IRepositoryBase<Class, Guid>
    {
        Task<bool> CheckExists(string name, string code);
        IQueryable<Class> GetQueryable();
        Task<int> GetTotalClassesAsync(string searchTerm);
        Task<List<Class>> GetClassesByIdsAsync(List<Guid> ids);
        Task DeleteClassesAsync(List<Class> classes);
        Task<List<User>> GetUsersByIdsAsync(List<Guid> userIds);
        Task<Class?> GetClassUserAsync(Guid classId);
    }
}
