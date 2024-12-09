using EduPlanManager.Models.Entities;
using EduPlanManager.Models.ViewModels;
using EduPlanManager.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduPlanManager.Controllers
{
    public class StudentScheduleController(IStudentScheduleService studentScheduleService, UserManager<User> userManager) : Controller
    {
        private readonly IStudentScheduleService _studentScheduleService = studentScheduleService;
        private readonly UserManager<User> _userManager = userManager;

        public async Task<IActionResult> WeeklySchedule(int page = 1)
        {
            const int pageSize = 1; 
            var user = _userManager.GetUserAsync(User).Result;
            var allWeeklySchedules = await _studentScheduleService.GetWeeklySchedulesAsync(user.Id);
            var pagedSchedules = allWeeklySchedules.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new WeeklyScheduleViewModel
            {
                WeeklySchedule = pagedSchedules.FirstOrDefault(),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)allWeeklySchedules.Count / pageSize)
            };

            return View(viewModel);
        }

    }
}
