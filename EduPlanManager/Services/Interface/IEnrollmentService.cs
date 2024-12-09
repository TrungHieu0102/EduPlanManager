using EduPlanManager.Models.DTOs.Enrollment;
using EduPlanManager.Models.DTOs.Respone;
using EduPlanManager.Models.Entities;
using PagedList.Core;

namespace EduPlanManager.Services.Interface
{
    public interface IEnrollmentService
    {
        Task<ResultPage<EnrollmentListRespone>> GetEligibleSubjects(Guid studentId, int page, int pageSize, string searchTerm, DayOfWeekEnum? dayOfWeek, SessionEnum? session);
        Task<List<EnrollmentStudentListResponse>> GetEnrollmentsByStudentIdAsync(Guid studentId);
        Task<Result<string>> EnrollSubjects(List<EnrollmentRequest> requests);
        Task<Result<List<EnrollmentRequestDto>>> GetAllEnrollmentRequestsAsync();
        Task<Result<bool>> ApproveEnrollmentAsync(Guid enrollmentId);
        Task<Result<bool>> DeleteEnrollmentAsync(Guid enrollmentId, Guid studentId);
    }
}
