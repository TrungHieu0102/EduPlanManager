using EduPlanManager.Models.DTOs.Enrollment;
using PagedList.Core;

namespace EduPlanManager.Models.ViewModels
{
    public class EnrollmentViewModel
    {
        public List<EnrollmentStudentListResponse> RegisteredSubjects { get; set; }
        public List<EnrollmentListRespone> EligibleSubjects { get; set; }

        // Các thông tin phân trang
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }

    }
}
