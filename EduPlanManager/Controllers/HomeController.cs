using EduPlanManager.Extentions;
using Microsoft.AspNetCore.Mvc;

namespace EduPlanManager.Controllers
{
    [AuthenticateUser]

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
