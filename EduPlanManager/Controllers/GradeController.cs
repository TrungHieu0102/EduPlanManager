using EduPlanManager.Models.DTOs.Grade;
using EduPlanManager.Models.Entities;
using EduPlanManager.Models.ViewModels;
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
        public async Task<IActionResult> GetTeacherResponsibleGrades(string? studentName = null,Guid ?subjectId= null , Guid? academicTermId = null, Guid? classId = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return RedirectToAction("Login", "Auth");   
            }
            var (teacherSubjects, teacherAcademicTerms, teacherClasses) = await _gradeService.GetTeacherSubjectsAndAcademicTermsAsync(user.Id);

            var grades = await _gradeService.GetTeacherResponsibleGradesAsync(user.Id, subjectId, studentName, academicTermId, classId);

            var viewModel = new TeacherGradesViewModel
            {
                Grades = grades,
                AcademicTerms = teacherAcademicTerms ?? new List<AcademicTerm>(),
                Classes = teacherClasses,
                Subjects = teacherSubjects
            };

            return View(viewModel);
        }



    }
}
