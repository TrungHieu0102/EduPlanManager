using EduPlanManager.Models.DTOs.Enrollment;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> EligibleSubjects()
        {
            var user = await _userManager.GetUserAsync(User);
            var subjects = await _enrollmentService.GetEligibleSubjects(user!.Id);
            ViewBag.StudentId = user.Id;
            return View(subjects); 
        }
        [HttpPost]
        public async Task<IActionResult> Enroll(List<string> selectedSubjects, Guid studentId)
        {
            if (selectedSubjects == null || selectedSubjects.Count == 0)
            {
                TempData["ErrorMessage"] = "Bạn chưa chọn môn học nào để đăng ký.";              
                return RedirectToAction("EligibleSubjects", new { studentId });
            }

            // Tạo danh sách các EnrollmentRequest từ selectedSubjects
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

            // Gọi dịch vụ EnrollSubjects để đăng ký các môn học
            var result = await _enrollmentService.EnrollSubjects(enrollmentRequests);

            TempData["ErrorMessage"] = result;

            return RedirectToAction("EligibleSubjects", new { studentId });
        }


    }
}
