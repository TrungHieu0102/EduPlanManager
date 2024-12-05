using EduPlanManager.Models.DTOs.Enrollment;

namespace EduPlanManager.Services.Interface
{
    public interface IEnrollmentService
    {
        Task<List<EnrollmentListRespone>> GetEligibleSubjects(Guid studentId);
        Task<string> EnrollSubjects(List<EnrollmentRequest> requests);
    }
}
