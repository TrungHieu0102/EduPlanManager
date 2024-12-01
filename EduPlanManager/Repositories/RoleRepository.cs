using EduPlanManager.Data;
using EduPlanManager.Models.Entities;
using EduPlanManager.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace EduPlanManager.Repositories
{
    public class RoleRepository(ApplicationDbContext context) : IRoleRepository
    {
       
        public async Task<Role> GetByNameAsync(string roleName)
        {
            return await context.Roles.SingleOrDefaultAsync(r => r.Name == roleName);
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await context.Roles.ToListAsync();
        }
    }
}
