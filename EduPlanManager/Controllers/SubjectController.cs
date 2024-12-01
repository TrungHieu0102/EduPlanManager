using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EduPlanManager.Controllers
{
    public class SubjectController : Controller
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm, int? semester, int? year, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _subjectService.SearchSubjectsAsync(searchTerm, semester, year, pageNumber, pageSize);
            Console.WriteLine(result.Message);
            if (result.IsSuccess)
            {
                ViewBag.TotalPages = result.TotalPages;
                ViewBag.CurrentPage = pageNumber;
                ViewData["SearchTerm"] = searchTerm;
                ViewData["Semester"] = semester;
                ViewData["Year"] = year;

                return View(result.Data);
            }
            return NoContent();
        }

        [HttpGet("Subject/Details/{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _subjectService.GetSubjectWithDetailsAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound();
            }
            return View(result.Data);
        }
        [HttpGet]
        public async Task<IActionResult> SearchSubjects(string term)
        {
            var result = await _subjectService.SearchSubjectsByNameOrCodeAsync(term);
            if(!result.IsSuccess)
            {
                return NotFound();
            }
            return Json(result.Data.Select(s => new { label = $"{s.Code} - {s.Name}", value = s.Id }));
        }
    }
}
