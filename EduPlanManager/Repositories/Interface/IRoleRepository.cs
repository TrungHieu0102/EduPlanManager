using EduPlanManager.Models.Entities;

namespace EduPlanManager.Repositories.Interface
{
    public interface IRoleRepository
    {
        Task<Role> GetByNameAsync(string roleName);
        Task<IEnumerable<Role>> GetAllAsync();
    }
}
