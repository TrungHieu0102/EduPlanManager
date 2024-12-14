using EduPlanManager.Models.DTOs.Grade;
using EduPlanManager.Models.Entities;
using EduPlanManager.Models.ViewModels;
using EduPlanManager.Services;
using EduPlanManager.Services.Interface;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Student")]
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
        [Authorize(Roles = "Student")]
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
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetTeacherResponsibleGrades(string? studentName = null,Guid ?subjectId= null , Guid? academicTermId = null, Guid? classId = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return RedirectToAction("Login", "Auth");   
            }
            var (teacherSubjects, teacherAcademicTerms) = await _gradeService.GetTeacherSubjectsAndAcademicTermsAsync(user.Id);

            var grades = await _gradeService.GetTeacherResponsibleGradesAsync(user.Id, subjectId, studentName, academicTermId, classId);

            var viewModel = new TeacherGradesViewModel
            {
                Grades = grades.Data,
                AcademicTerms = teacherAcademicTerms ?? new List<AcademicTerm>(),
                Subjects = teacherSubjects
            };

            return View(viewModel);
        }
        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public IActionResult AddGrade(Guid studentId, Guid subjectId)
        {
            ViewBag.StudentId = studentId;
            ViewBag.SubjectId = subjectId;

            return View();
        }
        [Authorize(Roles = "Teacher")]

        [HttpPost]
        public async Task<IActionResult> AddGrade(Guid studentId, Guid subjectId, GradeType gradeType, float score)
        {
            var user = await _userManager.GetUserAsync(User);
            if (gradeType == GradeType.Midterm || gradeType == GradeType.Final)
            {
                if (score < 0 || score > 10)
                {
                    ModelState.AddModelError("", "Điểm giữa kỳ và điểm cuối kỳ phải từ 0 đến 10.");
                    return View();
                }
            }
            else if (gradeType == GradeType.Bonus)
            {
                if (score < 0 || score > 4)
                {
                    ModelState.AddModelError("", "Điểm cộng phải từ 0 đến 4.");
                    return View();
                }
            }

            var result = await _gradeService.AddGradeAsync(user.Id,studentId, subjectId, score, gradeType);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Điểm đã được cập nhật thành công!";
                return RedirectToAction("GetTeacherResponsibleGrades");
            }
            TempData["ErrorMessage"] = result.Message;
            return View();
        }


    }
}
