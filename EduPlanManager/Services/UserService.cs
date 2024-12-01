using AutoMapper;
using EduPlanManager.Extentions;
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
        private readonly IPhotoService _photoService;


        private readonly IMapper _mapper;
        public UserService(IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper, RoleManager<Role> roleManager, IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
            _photoService = photoService;
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
                user.UserName = request.Email;
                var password = RandomStringGenerator.GenerateRandomString(10);
                var result = await _userManager.CreateAsync(user, password);
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
                    Message = password
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
        public async Task<Result<bool>> UpdateUserAsync(UpdateUserDto userUpdateDto, string userId)
        {
            var result = new Result<bool>
            {
                IsSuccess = false
            };

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                result.Message = "User not found.";
                return result;
            }

            user.FirstName = userUpdateDto.FirstName;
            user.LastName = userUpdateDto.LastName;
            user.Address = userUpdateDto.Address;
            user.DateOfBirth = userUpdateDto.DateOfBirth;
            user.Gender = userUpdateDto.Gender;
            user.PhoneNumber = userUpdateDto.Phone;
            user.Email = userUpdateDto.Email;

            if (userUpdateDto.ProfilePicture != null)
            {
                if (!string.IsNullOrEmpty(user.ProfilePicture))
                {
                    var publicId = PhotoExtensions.ExtractPublicId(user.ProfilePicture);
                    await _photoService.DeletePhotoAsync("profile_pictures", publicId);                
                }

                var uploadResult = await _photoService.AddPhotoAsync(userUpdateDto.ProfilePicture, "profile_pictures");

                if (uploadResult.Error == null)
                {
                    user.ProfilePicture = uploadResult.SecureUrl.ToString();
                }
                else
                {
                    result.Message = "Failed to upload the photo.";
                    return result;
                }
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (updateResult.Succeeded)
            {
                result.IsSuccess = true;
                result.Message = "User profile updated successfully.";
                result.Data = true; 
            }
            else
            {
                result.Message = "Failed to update user profile.";
            }

            return result;
        }
    }
}
