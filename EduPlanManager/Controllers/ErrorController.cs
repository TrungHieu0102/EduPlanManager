using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class ErrorController : Controller
{
    [AllowAnonymous]
    public IActionResult Forbidden()
    {
        Response.StatusCode = 403;
        return View();
    }

    [Route("Error/{code:int}")]
    public IActionResult GeneralError(int code)
    {
        Response.StatusCode = code;
        return View("Error", code);
    }
}
