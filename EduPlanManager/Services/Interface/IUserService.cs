using EduPlanManager.Models.DTOs.Respone;
using EduPlanManager.Models.DTOs.User;
using EduPlanManager.Models.Entities;

namespace EduPlanManager.Services.Interface
{
    public interface IUserService
    {
        Task<User> GetByEmailAsync(string email);
        Task<Result<UserDto>> CreateAsync(CreateUserDto request);
        Task<List<User>> GetAllUser();
    }
}
