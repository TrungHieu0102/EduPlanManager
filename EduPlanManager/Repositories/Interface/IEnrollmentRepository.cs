using EduPlanManager.Models.DTOs.Enrollment;
using EduPlanManager.Models.DTOs.SubjectSchedule;
using EduPlanManager.Models.Entities;

namespace EduPlanManager.Repositories.Interface
{
    public interface IEnrollmentRepository : IRepositoryBase<Enrollment, Guid>
    {
        Task<List<EnrollmentListRespone>> GetEligibleSubjects(Guid studentId);
        Task<bool> IsStudentInClass(Guid studentId, Guid subjectId);
        Task<bool> HasConflictSchedule(Guid studentId, Guid subjectScheduleId);
        Task<bool> HasDuplicateSubject(Guid studentId, Guid subjectId, Guid academicTermId);
        Task AddEnrollment(Enrollment enrollment);
    }
}
