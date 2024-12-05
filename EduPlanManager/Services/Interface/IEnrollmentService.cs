using EduPlanManager.Models.DTOs.Enrollment;
using EduPlanManager.Models.DTOs.Respone;
using PagedList.Core;

namespace EduPlanManager.Services.Interface
{
    public interface IEnrollmentService
    {
        Task<ResultPage<EnrollmentListRespone>> GetEligibleSubjects(Guid studentId, int page, int pageSize);        Task<List<EnrollmentDetailResponse>> GetAllEnrollmentDetailsAsync();
        Task<List<EnrollmentStudentListResponse>> GetEnrollmentsByStudentIdAsync(Guid studentId);
        Task<string> EnrollSubjects(List<EnrollmentRequest> requests);
    }
}
