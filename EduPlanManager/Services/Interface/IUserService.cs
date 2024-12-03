using EduPlanManager.Models.DTOs.Respone;
using EduPlanManager.Models.DTOs.User;
using EduPlanManager.Models.Entities;

namespace EduPlanManager.Services.Interface
{
    public interface IUserService
    {
        Task<User> GetByEmailAsync(string email);
        Task<Result<UserDto>> CreateAsync(CreateUserDto request);
        Task<Result<List<UserInListDTO>>> GetAllUser();
        Task<Result<bool>> UpdateUserAsync(UpdateUserDto userUpdateDto, string userId);
        Task<Result<UserDetailsDto>> GetUserWithRolesAsync(Guid userId);
        Task<List<UserInListDTO>> GetUsersWithoutClassAsync(Guid id);
        Task DeleteUsersAsync(List<Guid> ids);
    }

}
