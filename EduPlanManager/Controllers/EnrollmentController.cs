using EduPlanManager.Models.DTOs.Enrollment;
using EduPlanManager.Models.Entities;
using EduPlanManager.Models.ViewModels;
using EduPlanManager.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PagedList;
namespace EduPlanManager.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly UserManager<User> _userManager;
        public EnrollmentController(IEnrollmentService enrollmentService, UserManager<User> userManager)
        {
            _enrollmentService = enrollmentService;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> EligibleSubjects(int page = 1, int pageSize = 10)
        {
            var user = await _userManager.GetUserAsync(User);
            var registeredSubjects = await _enrollmentService.GetEnrollmentsByStudentIdAsync(user!.Id);

            // Lấy các môn học có thể đăng ký với phân trang
            var eligibleSubjectsResult = await _enrollmentService.GetEligibleSubjects(user.Id, page, pageSize);

            // Tạo ViewModel và gán dữ liệu
            var viewModel = new EnrollmentViewModel
            {
                RegisteredSubjects = registeredSubjects,
                EligibleSubjects = eligibleSubjectsResult.Data.ToList(), // Chỉ lấy danh sách môn học
                TotalCount = eligibleSubjectsResult.TotalCount ?? 0,
                TotalPages = eligibleSubjectsResult.TotalPages ?? 0,
                CurrentPage = eligibleSubjectsResult.CurrentPage ?? 1
            };

            // Truyền StudentId cho view
            ViewBag.StudentId = user.Id;

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Enroll(List<string> selectedSubjects, Guid studentId)
        {
            if (selectedSubjects == null || selectedSubjects.Count == 0)
            {
                TempData["ErrorMessage"] = "Bạn chưa chọn môn học nào để đăng ký.";              
                return RedirectToAction("EligibleSubjects", new { studentId });
            }

            List<EnrollmentRequest> enrollmentRequests = new List<EnrollmentRequest>();

            foreach (var subjectSchedule in selectedSubjects)
            {
                var parts = subjectSchedule.Split('|');
                var subjectId = Guid.Parse(parts[0]);
                var scheduleId = Guid.Parse(parts[1]);

                var request = new EnrollmentRequest
                {
                    StudentId = studentId,
                    SubjectId = subjectId,
                    SubjectScheduleId = scheduleId,
                    AcademicTermId = Guid.Parse("1CCB0087-DBD0-4BFA-B990-3F3CD481ACB0") 
                };

                enrollmentRequests.Add(request);
            }

            var result = await _enrollmentService.EnrollSubjects(enrollmentRequests);

            TempData["ErrorMessage"] = result;

            return RedirectToAction("EligibleSubjects", new { studentId });
        }


    }
}
