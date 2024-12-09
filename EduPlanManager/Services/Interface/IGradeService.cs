using EduPlanManager.Models.DTOs.Grade;
using EduPlanManager.Models.DTOs.Respone;

namespace EduPlanManager.Services.Interface
{
    public interface IGradeService
    {
        Task<Result<List<GradeDto>>> GetGradeByUserID(Guid userId);
        Task<List<SemesterGradeDto>> GetStudentGradesGroupedBySemesterAsync(Guid studentId);
    }
}
