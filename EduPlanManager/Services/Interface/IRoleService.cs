using EduPlanManager.Models.Entities;

namespace EduPlanManager.Services.Interface
{
    public interface IRoleService
    {
        Task<Role> GetByNameAsync(string roleName);
        Task<List<Role>> GetAllRole();
    }
}
