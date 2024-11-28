using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EduPlanManager.Extentions
{
    public class AuthenticateUserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var accessTokenCookies = context.HttpContext.Request.Cookies["AccessToken"];
            var accessTokenSession = context.HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrEmpty(accessTokenCookies) && string.IsNullOrEmpty(accessTokenSession))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
