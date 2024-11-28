using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using EduPlanManager.UnitOfWork;

namespace EduPlanManager.Services
{
    public class RoleService :IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Role> GetByNameAsync(string roleName)
        {
            return await _unitOfWork.Roles.GetByNameAsync(roleName);
        }
        public async Task<List<Role>> GetAllRole()
        {
            var roles= await _unitOfWork.Roles.GetAllAsync();
            return roles.ToList();
        }
    }
}
