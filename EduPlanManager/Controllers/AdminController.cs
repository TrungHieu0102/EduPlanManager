using EduPlanManager.Common.TemplateEmail;
using EduPlanManager.Models.DTOs.User;
using EduPlanManager.Services;
using EduPlanManager.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EduPlanManager.Controllers
{
    [Route("/admin")]
    [ValidateModelState]

    public class AdminController : Controller
    {

        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IEmailService _emailService;
        private readonly ISubjectService _subjectService;

        public AdminController(IUserService userService, IRoleService roleService, IEmailService emailService, ISubjectService subjectService)
        {
            _userService = userService;
            _roleService = roleService;
            _emailService = emailService;
            _subjectService = subjectService;
        }
        [HttpGet("users")]
        public async Task<IActionResult> Users()
        {
            var result = await _userService.GetAllUser();
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Không có user.";
            }
            return View(result.Data);
        }
        [HttpGet("create-user")]
        public async Task<IActionResult> CreateUser()
        {
            var roles = await _roleService.GetAllRole();
            ViewBag.Roles = roles;
            return View();
        }
        [HttpPost("create-user")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserDto request)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.CreateAsync(request);
                if (result.IsSuccess)
                {
                    string body = GenerateEmailBody.GetEmailOTPBody(request.Email, result.Message);
                    await _emailService.SendEmailAsync(request.Email!, "Tạo tài khoản thành công", body, true);
                    TempData["SuccessMessage"] = "Người dùng đã được tạo thành công.";
                    return RedirectToAction("Index", "Home");
                }
                TempData["ErrorMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin nhập vào.";
            }
            var roles = await _roleService.GetAllRole();
            ViewBag.Roles = roles;

            return View(request);

        }
        [HttpGet("update-user")]
        public async Task<IActionResult> Update(Guid id)
        {
            var result = await _userService.GetUserWithRolesAsync(id);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Người dùng không tồn tại.";
                return RedirectToAction("Index");
            }
          
            var userUpdateDto = new UpdateUserDto
            {
                Id = result.Data.Id,
                FirstName = result.Data.FirstName,
                LastName = result.Data.LastName,
                Email = result.Data.Email,
                Phone = result.Data.PhoneNumber,
                Address = result.Data.Address,
                Roles = result.Data.Roles,
            };
            return View(userUpdateDto);
        }
        [HttpPost("update-user")]
        public async Task<IActionResult> Update(UpdateUserDto request)
        {
            var resutl = await _userService.UpdateUserAsync(request, request.Id.ToString());
            if (!resutl.IsSuccess)
            {
                TempData["ErrorMessage"] = resutl.Message;
                return View(request);
            }
            TempData["SuccessMessage"] = "Cập nhật thành công.";
            return RedirectToAction("Users");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRange(string selectedIds)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedIds))
                {
                    throw new Exception("Chưa có người dùng nào được chọn");
                }

                var ids = selectedIds.Split(',').Select(id => Guid.Parse(id)).ToList();
                await _userService.DeleteUsersAsync(ids);
                TempData["SuccessMessage"] = "Xóa thành công";
                return RedirectToAction("Users");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
       
        [HttpGet("GetUsersWithoutClass")]
        public async Task<IActionResult> GetUsersWithoutClass(Guid id)
        {
            var users = await _userService.GetUsersWithoutClassAsync(id);

            if (users == null || users.Count == 0)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng không có lớp học.";
                return RedirectToAction("Index");
            }
            ViewData["ClassId"] = id;

            return View(users);
        }
        [HttpGet("GetSubjectWithoutClass")]
        public async Task<IActionResult> GetSubjectWithoutClass(Guid id)
        {
            var result = await _subjectService.GetSubjectsClassAsync(false, id);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Index");
            }
            ViewData["ClassId"] = id;
            return View(result.Data);
        }
    }
}
