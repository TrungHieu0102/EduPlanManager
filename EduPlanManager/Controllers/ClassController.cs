using EduPlanManager.Models.DTOs.Class;
using EduPlanManager.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EduPlanManager.Controllers
{
    public class ClassController : Controller
    {
        private readonly IClassService _classService;
        public ClassController(IClassService classService)
        {
            _classService = classService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _classService.SearchClassesAsync(searchTerm, pageNumber, pageSize);
            if (result.IsSuccess)
            {
                ViewBag.TotalPages = result.TotalPages;
                ViewBag.CurrentPage = pageNumber;
                ViewData["SearchTerm"] = searchTerm;
                return View(result.Data);
            }
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> SearchClasses(string term)
        {
            var result = await _classService.SearchClassesByNameOrCodeAsync(term);
            if (!result.IsSuccess)
            {
                return NotFound();
            }
            return Json(result.Data.Select(s => new { label = $"{s.Code} - {s.ClassName}" }));
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _classService.DeleteClassById(id);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Xoá thất bại";
                return NotFound();
            }
            TempData["SuccessMessage"] = "Xóa thành công";
            return RedirectToAction("Index");
        }
        [HttpGet("Class/Update/{id}")]
        public async Task<IActionResult> Update(Guid id)
        {
            var classEntity = await _classService.GetClass(id);
            if (!classEntity.IsSuccess)
            {
                TempData["ErrorMessage"] = classEntity.Message;
                return NotFound();
            }
            TempData["SuccessMessage"] = "Cập nhật thành công";

            return View(classEntity);
        }
        [HttpPost]
        public async Task<IActionResult> Update(CreateUpdateClassDTO classUpdateDTO)
        {

            var result = await _classService.UpdateClass(classUpdateDTO);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Cập nhật môn học thất bại";
                return View(classUpdateDTO);
            }

            TempData["SuccessMessage"] = "Cập nhật môn học thành công";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRange(string selectedIds)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedIds))
                {
                    throw new Exception("Chưa có môn học nào được chọn");
                }

                var ids = selectedIds.Split(',').Select(id => Guid.Parse(id)).ToList();
                await _classService.DeleteClassesAsync(ids);
                TempData["SuccessMessage"] = "Xóa thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpGet("create-class")]
        public IActionResult CreateSubject()
        {
            return View();
        }
        [HttpPost("create-class")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateClass(CreateUpdateClassDTO classRequest)
        {
            if (ModelState.IsValid)
            {
                var result = await _classService.CreateClassAsync(classRequest);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Môn học đã được tạo thành công.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin nhập vào.";
            }
            return View(classRequest);
        }
    }
}
