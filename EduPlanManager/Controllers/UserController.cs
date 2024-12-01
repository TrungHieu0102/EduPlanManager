using EduPlanManager.Models.DTOs.User;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services;
using EduPlanManager.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduPlanManager.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IPhotoService _photoService;
        private readonly IUserService _userService; 


        public UserController(UserManager<User> userManager,IPhotoService photoService,IUserService userService)
        {
            _userManager = userManager;
            _photoService = photoService;
            _userService = userService;
        }

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

            ModelState.AddModelError("", result.Message);
            TempData["ErrorMessage"] = result.Message;
            return View(userUpdateDto); 
        }

        public IActionResult ProfileUpdated()
        {
            return View();
        }
    }
}
