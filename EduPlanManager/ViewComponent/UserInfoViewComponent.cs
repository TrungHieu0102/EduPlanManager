using EduPlanManager.Models.DTOs.User;
using EduPlanManager.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduPlanManager
{
    public class UserInfoViewComponent : ViewComponent
    {
        private readonly UserManager<User> _userManager;

        public UserInfoViewComponent(UserManager<User> userManager)
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

                return View(userInfo);
            }

            return View(null);
        }
    }
}
