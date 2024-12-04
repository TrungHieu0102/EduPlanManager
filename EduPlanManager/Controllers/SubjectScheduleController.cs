using EduPlanManager.Extentions;
using EduPlanManager.Models.DTOs.SubjectSchedule;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services;
using EduPlanManager.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EduPlanManager.Controllers
{
    [ValidateModelState]

    public class SubjectScheduleController : Controller
    {
        private readonly ISubjectScheduleService _subjectScheduleService;
        private readonly ISubjectService _subjectService;

        public SubjectScheduleController(ISubjectScheduleService subjectScheduleService, ISubjectService subjectService)
        {
            _subjectScheduleService = subjectScheduleService;
            _subjectService = subjectService;
        }
        // GET: SubjectSchedule
        public async Task<IActionResult> Index()
        {
            var result = await _subjectScheduleService.GetAllSchedulesAsync();
            if (result.IsSuccess)
            {
                return View(result.Data);
            }

            TempData["ErrorMessage"] = result.Message;
            return View("Error");
        }
        // GET: SubjectSchedule/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _subjectScheduleService.GetScheduleByIdAsync(id);
            if (result.IsSuccess)
            {
                return View(result.Data);
            }

            TempData["ErrorMessage"] = result.Message;
            return View("Index");
        }
        // GET: SubjectSchedule/Create
        public IActionResult Create()
        {
            // Lấy giá trị của DayOfWeekEnum với DisplayAttribute
            ViewData["DayOfWeekEnum"] = Enum.GetValues(typeof(DayOfWeekEnum))
                .Cast<DayOfWeekEnum>()
                .Select(e => new { Id = (int)e, Name = e.GetDisplayName() })
                .ToList();

            // Lấy giá trị của SessionEnum với DisplayAttribute
            ViewData["SessionEnum"] = Enum.GetValues(typeof(SessionEnum))
                .Cast<SessionEnum>()
                .Select(e => new { Id = (int)e, Name = e.GetDisplayName() })
                .ToList();
            return View();
        }

        // POST: SubjectSchedule/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSubjectScheduleDTO dto)
        {


            dto.StartTime = TimeSpan.Parse(dto.StartTime.ToString());
            dto.EndTime = TimeSpan.Parse(dto.EndTime.ToString());
            var result = await _subjectScheduleService.CreateScheduleAsync(dto);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Lịch học đã được tạo thành công.";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = result.Message;

            ViewData["DayOfWeekEnum"] = Enum.GetValues(typeof(DayOfWeekEnum))
                .Cast<DayOfWeekEnum>()
                .Select(e => new { Id = (int)e, Name = e.GetDisplayName() })
                .ToList();

            ViewData["SessionEnum"] = Enum.GetValues(typeof(SessionEnum))
                .Cast<SessionEnum>()
                .Select(e => new { Id = (int)e, Name = e.GetDisplayName() })
                .ToList();
            return View();

        }
        // GET: SubjectSchedule/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            ViewData["DayOfWeekEnum"] = Enum.GetValues(typeof(DayOfWeekEnum))
                .Cast<DayOfWeekEnum>()
                .Select(e => new { Id = (int)e, Name = e.GetDisplayName() })
                .ToList();

            ViewData["SessionEnum"] = Enum.GetValues(typeof(SessionEnum))
                .Cast<SessionEnum>()
                .Select(e => new { Id = (int)e, Name = e.GetDisplayName() })
                .ToList();
            var result = await _subjectScheduleService.GetScheduleByIdAsync(id);
            if (result.IsSuccess)
            {
                var editDto = new UpdateSubjectScheduleDTO
                {
                    Id = result.Data.Id,
                    DayOfWeek = result.Data.DayOfWeek,
                    Session = result.Data.Session,
                    StartTime = result.Data.StartTime,
                    EndTime = result.Data.EndTime
                };
                return View(editDto);
            }


            return View("Error");
        }

        // POST: SubjectSchedule/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateSubjectScheduleDTO dto)
        {
            if (ModelState.IsValid)
            {
                var result = await _subjectScheduleService.UpdateScheduleAsync(dto);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Lịch học đã được cập nhật thành công.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = result.Message;
                ViewData["DayOfWeekEnum"] = Enum.GetValues(typeof(DayOfWeekEnum))
               .Cast<DayOfWeekEnum>()
               .Select(e => new { Id = (int)e, Name = e.GetDisplayName() })
               .ToList();

                ViewData["SessionEnum"] = Enum.GetValues(typeof(SessionEnum))
                    .Cast<SessionEnum>()
                    .Select(e => new { Id = (int)e, Name = e.GetDisplayName() })
                    .ToList();
                return View(dto);
            }

            return View(dto);
        }



        // POST: SubjectSchedule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var result = await _subjectScheduleService.DeleteScheduleAsync(id);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Lịch học đã được xóa thành công.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = result.Message;
            return View("Error");
        }
        [HttpGet("GetSubjectWithoutSchedule")]
        public async Task<IActionResult> GetSubjectWithoutSchedule(Guid id)
        {
            var result = await _subjectService.GetSubjectsScheduleAsync(false, id);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Index");
            }
            ViewData["ScheduleId"] = id;
            return View(result.Data);
        }
        [HttpPost("AddSubjectsToSchedule")]
        public async Task<IActionResult> AddSubjectsToSchedule(AddToScheduleDTO<Subject> dto)
        {
            var result = await _subjectScheduleService.AddSubjectsToSchedule(dto.Ids, dto.ScheduleId);

            if (result)
                return RedirectToAction("Index");
            else
                TempData["ErrorMessage"] = "Không thể thêm người dùng vào lớp.";
            return RedirectToAction("Index");
        }
    }
}
