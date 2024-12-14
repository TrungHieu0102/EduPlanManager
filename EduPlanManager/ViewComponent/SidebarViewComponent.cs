using EduPlanManager.Models.DTOs.User;
using EduPlanManager.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduPlanManager.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        private readonly UserManager<User> _userManager;

        public SidebarViewComponent(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user != null)
            {
                var userInfo = new UserInfoDto
                {
                    FullName = user.GetFullName(),
                    ProfilePictureUrl = user.ProfilePicture
                };

                var role = await _userManager.GetRolesAsync(user);
                if (role.Contains("Admin"))
                {
                    return View("AdminSidebar", userInfo);
                }
                else if (role.Contains("Teacher"))
                {
                    return View("TeacherSidebar", userInfo);
                }
                else if (role.Contains("Student"))
                {
                    return View("StudentSidebar", userInfo);
                }
            }

            // Nếu người dùng chưa đăng nhập, trả về sidebar mặc định
            return View("DefaultSidebar");
        }
    }
}
