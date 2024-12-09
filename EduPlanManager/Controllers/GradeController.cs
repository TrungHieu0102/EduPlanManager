using EduPlanManager.Models.DTOs.Grade;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services;
using EduPlanManager.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduPlanManager.Controllers
{
    public class GradeController : Controller
    {
        private readonly IGradeService _gradeService;
        private readonly UserManager<User> _userManager;
        public GradeController(IGradeService gradeService, UserManager<User> userManager)
        {
            _gradeService = gradeService;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var result = await _gradeService.GetGradeByUserID(user.Id);
            if (!result.IsSuccess)
            {
                ViewData["ErrorMessage"] = result.Message;
                return NoContent();
            }

            var groupedGrades = result.Data
                .GroupBy(g => g.SubjectName)
                .Select(g => new GroupedGradeDto
                {
                    SubjectName = g.Key,
                    Grades = g.ToList()
                })
                .ToList();

            return View(groupedGrades);
        }
        public async Task<IActionResult> StudentGrades()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var semesterGrades = await _gradeService.GetStudentGradesGroupedBySemesterAsync(user.Id);

            return View(semesterGrades);
        }


    }
}
