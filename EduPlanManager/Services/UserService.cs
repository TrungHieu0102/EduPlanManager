using AutoMapper;
using EduPlanManager.Models.DTOs.Respone;
using EduPlanManager.Models.DTOs.User;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using EduPlanManager.UnitOfWork;
using Microsoft.AspNetCore.Identity;

namespace EduPlanManager.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        private readonly IMapper _mapper;
        public UserService(IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper, RoleManager<Role> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _unitOfWork.Users.GetByEmailAsync(email);
        }

        public async Task<Result<UserDto>> CreateAsync(CreateUserDto request)
        {
            try
            {
                var exitsUser = await _unitOfWork.Users.GetByEmailAsync(request.Email);
                if (exitsUser != null)
                {
                    throw new Exception("Email already exists");
                }
                var user = _mapper.Map<User>(request);
                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.FirstOrDefault().Description);
                }
                if (!await _roleManager.RoleExistsAsync(request.Role))
                {
                    await _roleManager.CreateAsync(new Role { Name = request.Role, DisplayName = request.Role });
                }
                await _userManager.AddToRoleAsync(user, request.Role);
                var resultModel = _mapper.Map<UserDto>(user);
                return new Result<UserDto>
                {
                    IsSuccess = true,
                    Data = resultModel,
                };
            }
            catch (Exception e)
            {
                return new Result<UserDto>
                {
                    IsSuccess = false,
                    Message = e.Message,
                };
            }
        }
        public async Task<List<User>> GetAllUser()
        {
            var user = await _unitOfWork.Users.GetAllAsync();
            return user.ToList();
        }
    }
}
