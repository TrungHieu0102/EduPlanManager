using EduPlanManager.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EduPlanManager.Filters
{
    public class MyAuthenFilter : Attribute, IAuthorizationFilter
    {
        private readonly UserManager<User> _userManager;

        public MyAuthenFilter(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Kiểm tra nếu người dùng đã đăng nhập
            var user = context.HttpContext.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                // Nếu chưa đăng nhập, trả về lỗi 401 (Unauthorized)
                context.Result = new UnauthorizedResult();
                return;
            }

            // Kiểm tra quyền hoặc logic khác nếu cần
            var userId = user.Identity.Name; // Hoặc cách lấy user ID khác
            var appUser = _userManager.FindByIdAsync(userId).Result;

            if (appUser == null)
            {
                // Nếu không tìm thấy người dùng trong cơ sở dữ liệu, trả về lỗi
                context.Result = new UnauthorizedResult();
                return;
            }

            // Thêm logic kiểm tra quyền nếu cần
            // Ví dụ: Kiểm tra nếu người dùng không phải là admin, trả về AccessDenied
            if (!user.IsInRole("Admin"))
            {
                context.Result = new ForbidResult(); // Trả về lỗi 403 (Access Denied)
                return;
            }
        }
    }
}
