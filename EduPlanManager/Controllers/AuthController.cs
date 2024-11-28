using EduPlanManager.Models.DTOs.Auth;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduPlanManager.Controllers
{
    [Route("/auth")]
    public class AuthController : Controller
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailService _emailService;

        public AuthController(ITokenService tokenService, UserManager<User> userManager, SignInManager<User> signInManager, IEmailService emailService)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var accessToken = _tokenService.GenerateAccessToken(user);
                    var refreshToken = _tokenService.GenerateRefreshToken();

                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
                    await _userManager.UpdateAsync(user);
                                        var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

                    if (model.RememberMe)
                    {
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            Expires = DateTime.Now.AddDays(30) 
                        };

                        Response.Cookies.Append("AccessToken", accessToken, cookieOptions);
                        Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);
                        Response.Cookies.Append("UserRole", role!, cookieOptions);

                    }
                    else
                    {
                        HttpContext.Session.SetString("AccessToken", accessToken);
                        HttpContext.Session.SetString("RefreshToken", refreshToken);
                        HttpContext.Session.SetString("UserRole", role!);
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            return View(model);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Remove("AccessToken");
            HttpContext.Session.Remove("RefreshToken");
            Response.Cookies.Delete("AccessToken");
            Response.Cookies.Delete("RefreshToken");
            return RedirectToAction("Login", "Auth");
        }
        [HttpPost]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = HttpContext.Session.GetString("RefreshToken");
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user != null && user.RefreshTokenExpiryTime > DateTime.Now)
            {
                var newAccessToken = _tokenService.GenerateAccessToken(user);
                HttpContext.Session.SetString("AccessToken", newAccessToken);
                return Ok(new { AccessToken = newAccessToken });
            }
            return Unauthorized();
        }
        [HttpGet("forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetLink = Url.Action("ResetPassword", "Auth", new { token = token, email = user.Email }, Request.Scheme);

                    await _emailService.SendConfirmationEmail(user.Email, user, resetLink);

                    TempData["SuccessMessage"] = "Chúng tôi đã gửi một link reset mật khẩu vào email của bạn.";
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError(string.Empty, "Không tìm thấy tài khoản với email này.");
            }
            return View(model);
        }
        [HttpGet("reset-password")]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                return BadRequest("Yêu cầu không hợp lệ.");
            }

            var model = new ResetPasswordDto { Token = token, Email = email };
            return View(model);
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Không tìm thấy tài khoản.");
                    return View(model);
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Mật khẩu của bạn đã được thay đổi thành công.";
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
    }
}
