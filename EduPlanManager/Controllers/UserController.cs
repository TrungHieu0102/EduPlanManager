using EduPlanManager.Models.DTOs.User;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduPlanManager.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IPhotoService _photoService;
        private readonly IUserService _userService;
        public UserController(UserManager<User> userManager, IPhotoService photoService, IUserService userService)
        {
            _userManager = userManager;
            _photoService = photoService;
            _userService = userService;
        }
        [Authorize]

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                return View(user);
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Update()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var userUpdateDto = new UpdateUserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Email = user.Email,
                Phone = user.PhoneNumber,
                PictureUrl = user.ProfilePicture
            };

            return View(userUpdateDto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(UpdateUserDto userUpdateDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userService.UpdateUserAsync(userUpdateDto, user.Id.ToString());

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Cập nhật thành công";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = result.Message;
            return View(userUpdateDto);
        }

        public IActionResult ProfileUpdated()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListUser()
        {
            var users = await _userManager.Users.ToListAsync();

            var nonAdminUsers = new List<User>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (!roles.Contains("Admin"))
                {
                    nonAdminUsers.Add(user);
                }
            }

            return View(nonAdminUsers);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Xóa người dùng thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa người dùng.";
            }

            return RedirectToAction("ListUser");
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(List<string> selectedUserIds)
        {
            if (selectedUserIds == null || selectedUserIds.Count == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một người dùng để xóa!";
                return RedirectToAction("Index");
            }

            foreach (var userId in selectedUserIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa người dùng!";
                        return RedirectToAction("Index");
                    }
                }
            }

            TempData["SuccessMessage"] = "Đã xóa người dùng thành công!";
            return RedirectToAction("ListUser");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Detail(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userDetailDto = new UserDetailsDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            };

            return View(userDetailDto);
        }
    }
}
