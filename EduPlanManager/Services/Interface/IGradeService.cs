using EduPlanManager.Models.DTOs.Grade;
using EduPlanManager.Models.DTOs.Respone;
using EduPlanManager.Models.Entities;

namespace EduPlanManager.Services.Interface
{
    public interface IGradeService
    {
        Task<Result<List<GradeDto>>> GetGradeByUserID(Guid userId);
        Task<List<SemesterGradeDto>> GetStudentGradesGroupedBySemesterAsync(Guid studentId);
        Task<Result<List<StudentSubjectGradeDto>>> GetTeacherResponsibleGradesAsync(Guid teacherId,Guid? subjectId, string? studentName = null, Guid? academicTermId = null, Guid? classId = null);
        Task<(List<Subject> Subjects, List<AcademicTerm> AcademicTerms)> GetTeacherSubjectsAndAcademicTermsAsync(Guid teacherId);
        Task<Result<bool>> AddGradeAsync(Guid teacherId, Guid studentId, Guid subjectId, float score, GradeType type);
    }
}
