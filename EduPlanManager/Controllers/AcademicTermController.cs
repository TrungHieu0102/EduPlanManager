using AutoMapper;
using EduPlanManager.Models.DTOs.AcademicTerm;
using EduPlanManager.Services.Interface;
using EduPlanManager.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduPlanManager.Controllers
{
    [ValidateModelState]
    public class AcademicTermController(IUnitOfWork unitOfWork, IAcademicTermService academicTermService, IMapper mapper) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAcademicTermService _academicTermService = academicTermService;
        private readonly IMapper _mapper = mapper;
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var academicTerms = await _unitOfWork.AcademicTerms.GetAllAsync();
            return View(academicTerms);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateUpdateAcademicTermDTO());
        }
        [Authorize(Roles = "Admin")]

        [HttpPost]
        public async Task<IActionResult> Create(CreateUpdateAcademicTermDTO request)
        {
            var result = await _academicTermService.CreateAcademicTermsAsync(request);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Create");

            }
            TempData["SuccessMessage"] = "Cập nhật thành công";
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _academicTermService.DeleteAcademicAsync(id);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
            }
            TempData["SuccessMessage"] = "Xóa thành công";
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var academicTerm = await _unitOfWork.AcademicTerms.GetByIdAsync(id);
            var response = _mapper.Map<CreateUpdateAcademicTermDTO>(academicTerm);
            return View(response);
        }
        [Authorize(Roles = "Admin")]

        [HttpPost]
        public async Task<IActionResult> Update(CreateUpdateAcademicTermDTO request)
        {
            var result = await _academicTermService.UpdateAcademic(request);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return View(request);
            }
            TempData["SuccessMessage"] = "Cập nhật thành công";
            return RedirectToAction("Index");
        }


    }
}
