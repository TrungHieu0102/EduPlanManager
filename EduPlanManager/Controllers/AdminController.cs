using EduPlanManager.Common.TemplateEmail;
using EduPlanManager.Models.DTOs.User;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services;
using EduPlanManager.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EduPlanManager.Controllers
{
    [Route("/admin")]
    public class AdminController : Controller
    {

        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IEmailService _emailService;

        public AdminController(IUserService userService, IRoleService roleService, IEmailService emailService)
        {
            _userService = userService;
            _roleService = roleService;
            _emailService = emailService;
        }
        [HttpGet("users")]
        public async Task<IActionResult> Users()
        {
            var users = await _userService.GetAllUser();
            return View(users);
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
        public async Task<IActionResult> CreateUser( CreateUserDto request)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.CreateAsync(request);
                if (result.IsSuccess)
                {
                    string body = GenerateEmailBody.GetEmailOTPBody(request.Email, request.Password);
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
    }
}
