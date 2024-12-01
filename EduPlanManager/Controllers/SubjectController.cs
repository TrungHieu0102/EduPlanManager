﻿using AutoMapper;
using EduPlanManager.Models.DTOs.Subject;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduPlanManager.Controllers
{
    public class SubjectController : Controller
    {
        private readonly ISubjectService _subjectService;
        private readonly ISubjectCategoryService _subjectCategoryService;
        private readonly IAcademicTermService _academicTermService;
        private readonly IMapper _mapper;
        public SubjectController(ISubjectService subjectService , IMapper mapper, ISubjectCategoryService subjectCategoryService, IAcademicTermService academicTermService)
        {
            _subjectService = subjectService;
            _subjectCategoryService = subjectCategoryService;
            _academicTermService = academicTermService;
            _mapper = mapper;
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
            if (!result.IsSuccess)
            {
                return NotFound();
            }
            return Json(result.Data.Select(s => new { label = $"{s.Code} - {s.Name}", value = s.Id }));
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _subjectService.DeleteSubjectById(id);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Xoá thất bại";
                return NotFound();
            }
            TempData["SuccessMessage"] = "Xóa thành công";
            return RedirectToAction("Index");
        }
        // GET: Subject/Update/{id}
        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var subject = await _subjectService.GetSubject(id);
            if (!subject.IsSuccess)
            {
                TempData["ErrorMessage"] = subject.Message;
                return NotFound();
            }

            var subjectUpdateDTO = _mapper.Map<SubjectUpdateDTO>(subject.Data);

            var categories = await _subjectCategoryService.GetAllCategoriesAsync();
            var academicTerms = await _academicTermService.GetAllAcademicTermsAsync();

            ViewBag.Categories = new SelectList(categories, "Id", "FullName", subjectUpdateDTO.CategoryId);
            ViewBag.AcademicTerms = new SelectList(academicTerms.Select(at => new
            {
                Id = at.Id,
                Text = $"Năm: {at.Year} - Học kì: {at.Semester}",
                Year = at.Year,
                Semester = at.Semester
            }), "Id", "Text", subjectUpdateDTO.AcademicTermId);
            return View(subjectUpdateDTO);
        }
        [HttpPost]
        public async Task<IActionResult> Update(SubjectUpdateDTO subjectUpdateDTO)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _subjectCategoryService.GetAllCategoriesAsync();
                var academicTerms = await _academicTermService.GetAllAcademicTermsAsync();

                ViewBag.Categories = new SelectList(categories, "Id", "FullName", subjectUpdateDTO.CategoryId);
                ViewBag.AcademicTerms = new SelectList(academicTerms.Select(at => new
                {
                    Id = at.Id,
                    Text = $"Năm: {at.Year} - Học kì: {at.Semester}",
                    Year = at.Year,
                    Semester = at.Semester
                }), "Id", "Text", subjectUpdateDTO.AcademicTermId);
                return View(subjectUpdateDTO);
            }

            var subject = _mapper.Map<Subject>(subjectUpdateDTO);

            var result = await _subjectService.UpdateSubject(subject);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Cập nhật môn học thất bại";
                return View(subjectUpdateDTO);
            }

            TempData["SuccessMessage"] = "Cập nhật môn học thành công";
            return RedirectToAction("Index");
        }
    }
}