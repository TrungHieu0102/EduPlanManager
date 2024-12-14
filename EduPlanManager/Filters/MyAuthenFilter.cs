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
            var user = context.HttpContext.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userId = user.Identity.Name; 
            var appUser = _userManager.FindByIdAsync(userId).Result;

            if (appUser == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (!user.IsInRole("Admin"))
            {
                context.Result = new ForbidResult(); 
                return;
            }
        }
    }
}
