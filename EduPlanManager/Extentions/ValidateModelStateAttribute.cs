using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ValidateModelStateAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var controller = context.Controller as Controller;
            if (controller != null)
            {
                context.Result = new ViewResult
                {
                    ViewData = controller.ViewData,
                    StatusCode = 400 
                };
            }
        }
    }
}
